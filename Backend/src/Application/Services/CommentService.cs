using Comments.Application.Interfaces.Repositories;
using Comments.Application.Interfaces.Services;
using Comments.Application.Mappers;
using Comments.Contracts;
using Comments.Contracts.Enums;
using Comments.Contracts.Filters;
using Comments.Domain;
using Comments.Infrastructure.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Comments.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IImageService _imageService;
        private readonly ITextFileService _textFileService;
        private readonly ILogger<CommentService> _logger;
        private readonly IDistributedCache _cache;

        private static bool _redisAvailable = true;
        private static DateTime? _redisDisabledUntil = null;

        private enum FileType
        {
            Image,
            Text,
            Unknown
        }

        public CommentService(ICommentRepository commentRepository, IImageService imageService, ITextFileService textFileService, ILogger<CommentService> logger, IDistributedCache cache)
        {
            _commentRepository = commentRepository;
            _imageService = imageService;
            _textFileService = textFileService;
            _logger = logger;
            _cache = cache;
        }

        public async Task<CommentResponse> CreateCommentAsync(CommentRequest request, CancellationToken cancellationToken, IFormFile? file = null)
        {
            if (request.ParentId.HasValue)
            {
                bool isParentId = await _commentRepository.ParentExistsAsync(request.ParentId.Value, cancellationToken);
                if (!isParentId)
                {
                    throw new KeyNotFoundException("Parent comment not found");
                }
            }

            Guid? imageId = null;
            Guid? textFileId = null;
            string? originalTextFileName = null;

            if (file != null)
            {
                var fileType = DetectFile(file);

                switch (fileType)
                {
                    case FileType.Image:
                        {
                            imageId = await _imageService.ProcessAndSaveImageAsync(file, cancellationToken);
                            break;
                        }

                    case FileType.Text:
                        {
                            var (fileId, originalFileName) = await _textFileService.ProcessAndSaveTextFileAsync(file, cancellationToken);
                            textFileId = fileId;
                            originalTextFileName = originalFileName;
                            break;
                        }

                    default:
                        throw new ArgumentException("Unsupported file type");
                }
            }

            // Sanitize request fields
            InputSanitizationService service = new InputSanitizationService();

            var cleanText = service.SanitizeComment(request.Text);
            var cleanUserName = service.SanitizeUsername(request.UserName);
            var cleanEmail = service.SanitizeEmail(request.Email);

            var comment = new Comment(
                request.ParentId,
                cleanUserName,
                cleanEmail,
                cleanText,
                imageId,
                textFileId,
                originalTextFileName
            );

            await _commentRepository.AddAsync(comment, cancellationToken);

            var imagePreviewUrl = _imageService.GetImagePreviewUrl(comment.ImageId);
            var imageOriginalUrl = _imageService.GetImageOriginalUrl(comment.ImageId);

            var commentResponse = CommentMapper.ToResponse(comment, imagePreviewUrl, imageOriginalUrl);

            _logger.LogAuditUser(
                "Created comment {CommentId}, UserName: {UserName}, HasFile: {HasFile}, ParentId: {ParentId}",
                commentResponse.Id,
                commentResponse.UserName,
                file != null,
                request.ParentId);

            return commentResponse;
        }

        public async Task<List<CommentResponse>> GetCommentsAsync(CommentQuery commentQuery, CancellationToken cancellationToken, int? parentId = null)
        {
            bool useKeyset = commentQuery.SortBy == CommentSortField.createdAt;

            var pageNumber= (commentQuery.Skip / commentQuery.PageSize) + 1;

            // cache only default page comments with sorting by createdAt in ASC order, without parentId ( for root comments)
            bool isCacheable =
                useKeyset &&
                parentId == null &&
                commentQuery.Ascending == true &&
                pageNumber <= 3; // limit caching to the first 3 pages

            if (isCacheable)
            {
                return await GetCachedCommentsAsync(commentQuery, cancellationToken);
            }

            var rawComments = useKeyset
                ? await _commentRepository.GetListKeysetAsync(commentQuery, cancellationToken, parentId)
                : await _commentRepository.GetListOffsetAsync(commentQuery, cancellationToken, parentId);

            var commentsResponse = rawComments
            .Select(c => CommentMapper.FromRaw(
                c,
                _imageService.GetImagePreviewUrl(c.ImageId),
                _imageService.GetImageOriginalUrl(c.ImageId)))
            .ToList();

            return commentsResponse;
        }

        public async Task<CommentResponse> GetCommentById(int id, CancellationToken cancellationToken)
        {
            var rawComment = await _commentRepository.GetByIdAsync(id, cancellationToken);

            if (rawComment == null)
                throw new KeyNotFoundException($"Comment for id: {id} not found");

            var imagePreviewUrl = _imageService.GetImagePreviewUrl(rawComment.ImageId);
            var ImageOriginalUrl = _imageService.GetImageOriginalUrl(rawComment.ImageId);

            var commentResponse = CommentMapper.FromRaw(rawComment, imagePreviewUrl, ImageOriginalUrl);

            return commentResponse;
        }

        private async Task<List<CommentResponse>> GetCachedCommentsAsync(CommentQuery commentQuery, CancellationToken cancellationToken)
        {
            //Redis availability check - every 30 seconds, if Redis is unavailable, immediately load from the DB until the lock time expires
            if (_redisAvailable || DateTime.UtcNow >= _redisDisabledUntil)
            {
                var pageNumber = (commentQuery.Skip / commentQuery.PageSize) + 1;

                var cacheKey = $"comments:page:{pageNumber}";

                try
                {
                    var cachedJson = await _cache.GetStringAsync(cacheKey, cancellationToken);

                    if (cachedJson != null)
                    {
                        _logger.LogInformation("CACHE HIT: {CacheKey}", cacheKey);

                        var cached = JsonSerializer.Deserialize<List<CommentResponse>>(cachedJson);

                        if (cached != null)
                            return cached;
                    }
                }
                catch (Exception ex)
                {
                    _redisAvailable = false;
                    _redisDisabledUntil = DateTime.UtcNow.AddSeconds(30);

                    _logger.LogWarning(ex, "Redis GET failed for key: {CacheKey}", cacheKey);
                }

                _logger.LogInformation("CACHE MISS: {CacheKey} - loading from DB", cacheKey);

                var rawComments = await _commentRepository.GetListKeysetAsync(commentQuery, cancellationToken);

                var commentsResponse = rawComments
                    .Select(c => CommentMapper.FromRaw(
                        c,
                        _imageService.GetImagePreviewUrl(c.ImageId),
                        _imageService.GetImageOriginalUrl(c.ImageId)))
                    .ToList();

                try
                {
                    var serialized = JsonSerializer.Serialize(commentsResponse);

                    await _cache.SetStringAsync(
                        cacheKey,
                        serialized,
                        new DistributedCacheEntryOptions
                        {
                            AbsoluteExpirationRelativeToNow =
                                TimeSpan.FromSeconds(20)
                        },
                        cancellationToken);

                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Redis SET failed for key: {CacheKey}", cacheKey);
                }

                return commentsResponse;
            }

            _logger.LogInformation("Redis unavailable until {UnavailableUntil}, loading from DB", _redisDisabledUntil);

            // fallback to DB, if Redis is unavailable 
            var fallbackRawComments = await _commentRepository.GetListKeysetAsync(commentQuery, cancellationToken);

            var fallbackcommentsResponse = fallbackRawComments
                .Select(c => CommentMapper.FromRaw(
                    c,
                    _imageService.GetImagePreviewUrl(c.ImageId),
                    _imageService.GetImageOriginalUrl(c.ImageId)))
                .ToList();

            return fallbackcommentsResponse;
        }

        private FileType DetectFile(IFormFile file)
        {
            var type = file.ContentType;

            if (type.StartsWith("image/"))
                return FileType.Image;

            if (type.StartsWith("text/"))
                return FileType.Text;

            return FileType.Unknown;
        }
    }
}
