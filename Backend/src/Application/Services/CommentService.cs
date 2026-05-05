using Comments.Application.Interfaces;
using Comments.Application.Mappers;
using Comments.Contracts;
using Comments.Infrastructure.Data;
using Comments.Infrastructure.Logging;
using Comments.Models;
using Comments.Models.Enums;
using Comments.Models.Filters;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Comments.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly CommentsDbContext _dbContext;
        private readonly ImageService _imageService;
        private readonly TextFileService _textFileService;
        private readonly ILogger<CommentService> _logger;
        public CommentService(CommentsDbContext dbContext, ImageService imageService, TextFileService textFileService, ILogger<CommentService> logger)
        {
            _dbContext = dbContext;
            _imageService = imageService;
            _textFileService = textFileService;
            _logger = logger;
        }

        public async Task<CommentResponse> CreateCommentAsync(CommentRequest request, CancellationToken cancellationToken, IFormFile? file = null)
        {
            if (request.ParentId.HasValue)
            {
                bool isParentId = await _dbContext.Comments.AnyAsync(c => c.Id == request.ParentId, cancellationToken);
                if (!isParentId)
                {
                    throw new KeyNotFoundException("Parent comment not found");
                }
            }

            Guid? imageId = null;
            Guid? textFileId = null;
            string? originalTextFileName = null;

            if (file?.Length > 0)
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
            var cleanText = InputSanitizationService.SanitizeComment(request.Text);
            var cleanUserName = InputSanitizationService.SanitizeUsername(request.UserName);
            var cleanEmail = InputSanitizationService.SanitizeEmail(request.Email);

            var comment = new Comment(
                request.ParentId,
                cleanUserName,
                cleanEmail,
                cleanText,
                imageId,
                textFileId,
                originalTextFileName
            );


            _dbContext.Add(comment);
            await _dbContext.SaveChangesAsync(cancellationToken);

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
            // for createdAt - using keyset pagination
            if (commentQuery.SortBy == CommentSortField.createdAt)
            {
               return await GetCommentsKeysetAsync(commentQuery, cancellationToken, parentId);
            }

            Console.WriteLine("Using OFFSET pagination");

            var comments = _dbContext.Comments
                .AsNoTracking()
                .AsQueryable();

            // filter by parentId (null for parent comments, or specific parentId for replies)
            comments = parentId == null
                ? comments.Where(c => c.ParentId == null)
                : comments.Where(c => c.ParentId == parentId);

            // sorting use OFFSET pagination
            comments = commentQuery.SortBy switch
            {
                CommentSortField.userName => commentQuery.Ascending
                    ? comments.OrderBy(c => c.UserName)
                    : comments.OrderByDescending(c => c.UserName),

                CommentSortField.email => commentQuery.Ascending
                    ? comments.OrderBy(c => c.Email)
                    : comments.OrderByDescending(c => c.Email),

                    _ => comments.OrderBy(c => c.CreatedAt) //как заглушка, т.к. сортировка по несуществующему полю уже проверяется на уровне контроллера
            };

            var rawComments = await comments
                .Skip(commentQuery.Skip)
                .Take(commentQuery.PageSize)
                .Select(c => new CommentRawDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageId = c.ImageId,
                    TextFileId = c.TextFileId,
                    OriginalTextFileName = c.OriginalTextFileName,
                    ReplyCount = parentId==null
                    ? c.Children.Count
                    : 0 // count the number of replies only for parent comments
                })
                .ToListAsync(cancellationToken);

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
            var rawComment = await _dbContext.Comments
                .Where(c => c.Id == id)
                .Select(c => new CommentRawDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageId = c.ImageId,
                    TextFileId = c.TextFileId,
                    OriginalTextFileName = c.OriginalTextFileName,
                   ReplyCount = c.Children.Count
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (rawComment == null)
                throw new KeyNotFoundException($"Comment for id: {id} not found");

            var imagePreviewUrl = _imageService.GetImagePreviewUrl(rawComment.ImageId);
            var ImageOriginalUrl = _imageService.GetImageOriginalUrl(rawComment.ImageId);

            var commentResponse = CommentMapper.FromRaw(rawComment, imagePreviewUrl, ImageOriginalUrl);

            return commentResponse;
        }

        // implement Keyset pagination only for field  - createdAt
        private async Task<List<CommentResponse>> GetCommentsKeysetAsync(CommentQuery commentQuery, CancellationToken cancellationToken, int? parentId = null)
        {
            var comments = _dbContext.Comments
                .AsNoTracking()
                .AsQueryable();

            // filter by parentId (null for parent comments, or specific parentId for replies)
            comments = parentId == null
                ? comments.Where(c => c.ParentId == null)
                : comments.Where(c => c.ParentId == parentId);

            if (commentQuery.CursorCreatedAt != null && commentQuery.CursorId != null)
            {   
                //move next page
                if (commentQuery.Sign==1)
                {
                    if(commentQuery.Ascending)
                    {
                        comments = comments.Where(c =>
                        c.CreatedAt > commentQuery.CursorCreatedAt ||
                        (c.CreatedAt == commentQuery.CursorCreatedAt && c.Id > commentQuery.CursorId));

                        comments = comments.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id);
                    }
                    else
                    {
                        comments = comments.Where(c =>
                        c.CreatedAt < commentQuery.CursorCreatedAt ||
                        (c.CreatedAt == commentQuery.CursorCreatedAt && c.Id < commentQuery.CursorId));

                        comments =  comments.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id);
                    }
                }
                //move previous page
                else
                {
                    if(commentQuery.Ascending)
                    {
                        comments = comments.Where(c =>
                        c.CreatedAt < commentQuery.CursorCreatedAt ||
                        (c.CreatedAt == commentQuery.CursorCreatedAt && c.Id < commentQuery.CursorId));

                        comments = comments.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id)
                        .Take(commentQuery.PageSize);

                        // возвращаем порядок под UI
                        comments = comments.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id);
                    }
                    else
                    {
                        comments = comments.Where(c =>
                        c.CreatedAt > commentQuery.CursorCreatedAt ||
                        (c.CreatedAt == commentQuery.CursorCreatedAt && c.Id > commentQuery.CursorId));

                        comments = comments.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id)
                        .Take(commentQuery.PageSize);

                        // возвращаем порядок под UI
                        comments = comments.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id);
                    }
                }
            }
            // On the first page, we sort by the createdAt and Id. For other pages,  will be using keyset pagination
            else
            {
                comments = commentQuery.Ascending
                    ? comments.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id)
                    : comments.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id);
            }

            var rawComments = await comments
                .Take(commentQuery.PageSize)
                .Select(c => new CommentRawDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageId = c.ImageId,
                    TextFileId = c.TextFileId,
                    OriginalTextFileName = c.OriginalTextFileName,
                    ReplyCount = parentId == null
                    ? c.Children.Count
                    : 0 // count the number of replies only for parent comments
                })
                .ToListAsync(cancellationToken);

            var commentsResponse = rawComments
            .Select(c => CommentMapper.FromRaw(
                c,
                _imageService.GetImagePreviewUrl(c.ImageId),
                _imageService.GetImageOriginalUrl(c.ImageId)))
            .ToList();

            return commentsResponse;
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
