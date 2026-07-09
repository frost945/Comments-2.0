using Comments.Application.Dtos;
using Comments.Application.Interfaces.Logging;
using Comments.Application.Interfaces.Repositories;
using Comments.Application.Interfaces.Sanitization;
using Comments.Application.Interfaces.Services;
using Comments.Application.Requests;
using Comments.Application.Services;
using Comments.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;

namespace tests.UnitTests.Comments.Application.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _repository = new();
        private readonly Mock<IImageService> _imageService = new();
        private readonly Mock<ITextFileService> _textFileService = new();
        private readonly Mock<IInputSanitizer> _sanitizer = new();
        private readonly Mock<ILogger<CommentService>> _logger = new();
        private readonly Mock<IDistributedCache> _cache = new();
        private readonly Mock<IAuditLogger> _auditLogger = new();

        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _sanitizer
               .Setup(x => x.SanitizeUsername(It.IsAny<string>()))
               .Returns((string s) => s);

            _sanitizer
                .Setup(x => x.SanitizeEmail(It.IsAny<string>()))
                .Returns((string s) => s);

            _sanitizer
                .Setup(x => x.SanitizeComment(It.IsAny<string>()))
                .Returns((string s) => s);

            _service = new CommentService(
                _repository.Object,
                _imageService.Object,
                _textFileService.Object,
                _sanitizer.Object,
                _logger.Object,
                _cache.Object,
                _auditLogger.Object);
        }

        [Fact]
        public async Task CreateCommentAsync_WithoutParent_CreatesComment()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            // Act
            CommentDto result = await _service.CreateCommentAsync(
                request,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(request.UserName, result.UserName);
            Assert.Equal(request.Text, result.Text);

            _repository.Verify(
                x => x.AddAsync(It.IsAny<Comment>(),
                It.IsAny<CancellationToken>()),
                Times.Once);

            _auditLogger.Verify(
                x => x.LogCommentCreated(
                    It.IsAny<int>(),
                    request.UserName,
                    false,
                    null),
                Times.Once);
        }

        [Fact]
        public async Task CreateCommentAsync_WithMissingParent_ThrowsKeyNotFoundException()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                ParentId = 10,
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            _repository
                .Setup(x => x.ParentExistsAsync(request.ParentId.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.CreateCommentAsync(request, CancellationToken.None));

            _repository.Verify(
                x => x.ParentExistsAsync(request.ParentId.Value, It.IsAny<CancellationToken>()),
                Times.Once);

            _repository.Verify(
                x => x.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _auditLogger.Verify(
                x => x.LogCommentCreated(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int?>()),
                Times.Never);

            _imageService.VerifyNoOtherCalls();
            _textFileService.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateCommentAsync_SanitizesAllInputFields()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "<b>John</b>",
                Email = "  JOHN@TEST.COM ",
                Text = "<script>alert(1)</script>Hello"
            };

            _sanitizer
                .Setup(x => x.SanitizeUsername(request.UserName))
                .Returns("John");

            _sanitizer
                .Setup(x => x.SanitizeEmail(request.Email))
                .Returns("john@test.com");

            _sanitizer
                .Setup(x => x.SanitizeComment(request.Text))
                .Returns("Hello");

            Comment? savedComment = null;

            _repository
                .Setup(x => x.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .Callback<Comment, CancellationToken>((comment, _) =>
                {
                    savedComment = comment;
                })
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateCommentAsync(
                request,
                CancellationToken.None);

            // Assert

            Assert.NotNull(savedComment);

            Assert.Equal("John", savedComment!.UserName);
            Assert.Equal("john@test.com", savedComment.Email);
            Assert.Equal("Hello", savedComment.Text);

            _sanitizer.Verify(x => x.SanitizeUsername(request.UserName), Times.Once);
            _sanitizer.Verify(x => x.SanitizeEmail(request.Email), Times.Once);
            _sanitizer.Verify(x => x.SanitizeComment(request.Text), Times.Once);
        }

        [Fact]
        public async Task CreateCommentAsync_CallsAuditLogger()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            // Act
            var result = await _service.CreateCommentAsync(
                request,
                CancellationToken.None);

            // Assert
            _auditLogger.Verify(x => x.LogCommentCreated(
                    result.Id,
                    result.UserName,
                    false,
                    null),
                Times.Once);
        }

        [Fact]
        public async Task CreateCommentAsync_WithImage_CallsImageService()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            var imageId = Guid.NewGuid();

            var file = new Mock<IFormFile>();
            file.Setup(x => x.FileName).Returns("image.jpg");
            file.Setup(x => x.ContentType).Returns("image/jpeg");

            _imageService
                .Setup(x => x.ProcessAndSaveAsync(file.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync(imageId);

            Comment? savedComment = null;

            _repository
                .Setup(x => x.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .Callback<Comment, CancellationToken>((comment, _) =>
                {
                    savedComment = comment;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _service.CreateCommentAsync(
                request,
                CancellationToken.None,
                file.Object);

            // Assert
            _imageService.Verify(
                x => x.ProcessAndSaveAsync(
                    file.Object,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _textFileService.Verify(
                x => x.ProcessAndSaveAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            Assert.NotNull(savedComment);
            Assert.Equal(imageId, savedComment!.ImageId);
            Assert.Null(savedComment.TextFileId);
        }

        [Fact]
        public async Task CreateCommentAsync_WithTextFile_CallsTextFileService()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            var textFileId = Guid.NewGuid();
            const string originalFileName = "notes.txt";

            var file = new Mock<IFormFile>();

            file.Setup(x => x.FileName).Returns(originalFileName);
            file.Setup(x => x.ContentType).Returns("text/plain");

            _textFileService
                .Setup(x => x.ProcessAndSaveAsync(file.Object, It.IsAny<CancellationToken>()))
                .ReturnsAsync((textFileId, originalFileName));

            Comment? savedComment = null;

            _repository
                .Setup(x => x.AddAsync(It.IsAny<Comment>(), It.IsAny<CancellationToken>()))
                .Callback<Comment, CancellationToken>((comment, _) =>
                {
                    savedComment = comment;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _service.CreateCommentAsync(
                request,
                CancellationToken.None,
                file.Object);

            // Assert
            _textFileService.Verify(
                x => x.ProcessAndSaveAsync(
                    file.Object,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _imageService.Verify(
                x => x.ProcessAndSaveAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            Assert.NotNull(savedComment);
            Assert.Equal(textFileId, savedComment!.TextFileId);
            Assert.Equal(originalFileName, savedComment.OriginalTextFileName);
            Assert.Null(savedComment.ImageId);
        }

        [Fact]
        public async Task CreateCommentAsync_WithUnsupportedFile_ThrowsArgumentException()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            var file = new Mock<IFormFile>();

            file.Setup(x => x.FileName).Returns("archive.zip");
            file.Setup(x => x.ContentType).Returns("application/zip");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.CreateCommentAsync(
                    request,
                    CancellationToken.None,
                    file.Object));

            Assert.Equal("Unsupported file type", exception.Message);

            _imageService.Verify(
                x => x.ProcessAndSaveAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _textFileService.Verify(
                x => x.ProcessAndSaveAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _repository.Verify(
                x => x.AddAsync(
                    It.IsAny<Comment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _auditLogger.Verify(
                x => x.LogCommentCreated(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int?>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateCommentAsync_WhenImageServiceThrows_PropagatesException()
        {
            // Arrange
            var request = new CreateCommentRequest
            {
                UserName = "John",
                Email = "john@test.com",
                Text = "Hello"
            };

            var file = new Mock<IFormFile>();

            file.Setup(x => x.FileName).Returns("image.jpg");
            file.Setup(x => x.ContentType).Returns("image/jpeg");

            _imageService
                .Setup(x => x.ProcessAndSaveAsync(
                    file.Object,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Image processing failed"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateCommentAsync(
                    request,
                    CancellationToken.None,
                    file.Object));

            Assert.Equal("Image processing failed", exception.Message);

            _repository.Verify(
                x => x.AddAsync(
                    It.IsAny<Comment>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            _auditLogger.Verify(
                x => x.LogCommentCreated(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int?>()),
                Times.Never);

            _textFileService.Verify(
                x => x.ProcessAndSaveAsync(
                    It.IsAny<IFormFile>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}
