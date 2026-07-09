

using Comments.Application.Queries.Enums;
using Comments.Application.Requests;
using Comments.Domain.Models;
using Comments.Infrastructure.Persistence;
using Comments.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using tests.IntegrationTests.Fixtures;

namespace tests.IntegrationTests.Persistence
{
    public class CommentRepositoryTests : IClassFixture<SqlServerFixture>
    {
        private readonly CommentsDbContext _context;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests(SqlServerFixture fixture)
        {
            _context = fixture.Context;
            _repository = new CommentRepository(_context);
        }

        [Fact]
        public async Task AddAsync_ShouldPersistComment()
        {
            // Arrange
            var comment = new Comment(
                null,
                "John",
                "john@test.com",
                "Hello",
                null,
                null,
                null);

            // Act
            await _repository.AddAsync(comment, CancellationToken.None);

            var before = await _context.Comments.CountAsync();
            Console.WriteLine(before);

            // Assert
            var saved = await _context.Comments
                .SingleAsync(c => c.Id == comment.Id);

            Assert.Equal("John", saved.UserName);
            Assert.Equal("Hello", saved.Text);
            Assert.NotEqual(0, saved.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCommentExists_ReturnsCommentDto()
        {
            // Arrange
            var comment = new Comment(
                null,
                "John",
                "john@test.com",
                "Hello world",
                null,
                null,
                null);

            await _repository.AddAsync(comment, CancellationToken.None);

            // Act
            var result = await _repository.GetByIdAsync(
                comment.Id,
                CancellationToken.None);

            // Assert
            Assert.NotNull(result);

            Assert.Equal(comment.Id, result!.Id);
            Assert.Equal(comment.UserName, result.UserName);
            Assert.Equal(comment.Text, result.Text);
            Assert.Equal(comment.ImageId, result.ImageId);
            Assert.Equal(comment.TextFileId, result.TextFileId);
            Assert.Equal(comment.OriginalTextFileName, result.TextFileName);
            Assert.Equal(0, result.ReplyCount);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCommentDoesNotExist_ReturnsNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(
                999999,
                CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ParentExistsAsync_WhenParentExists_ReturnsTrue()
        {
            // Arrange
            var parent = new Comment(
                null,
                "John",
                "john@test.com",
                "Parent comment",
                null,
                null,
                null);

            await _repository.AddAsync(parent, CancellationToken.None);

            // Act
            var result = await _repository.ParentExistsAsync(
                parent.Id,
                CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ParentExistsAsync_WhenParentDoesNotExist_ReturnsFalse()
        {
            // Act
            var result = await _repository.ParentExistsAsync(
                999999,
                CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ParentExistsAsync_WhenCommentIsReply_ReturnsFalse()
        {
            // Arrange
            var parent = new Comment(
                null,
                "John",
                "john@test.com",
                "Parent",
                null,
                null,
                null);

            await _repository.AddAsync(parent, CancellationToken.None);

            var reply = new Comment(
                parent.Id,
                "Mike",
                "mike@test.com",
                "Reply",
                null,
                null,
                null);

            await _repository.AddAsync(reply, CancellationToken.None);

            // Act
            var result = await _repository.ParentExistsAsync(
                reply.Id,
                CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenParentComment_DeletesParentAndReplies()
        {
            // Arrange
            var parent = new Comment(
                null,
                "John",
                "john@test.com",
                "Parent",
                null,
                null,
                null);

            await _repository.AddAsync(parent, CancellationToken.None);

            var reply1 = new Comment(
                parent.Id,
                "Mike",
                "mike@test.com",
                "Reply 1",
                null,
                null,
                null);

            var reply2 = new Comment(
                parent.Id,
                "Kate",
                "kate@test.com",
                "Reply 2",
                null,
                null,
                null);

            await _repository.AddAsync(reply1, CancellationToken.None);
            await _repository.AddAsync(reply2, CancellationToken.None);

            // Act
            var deleted = await _repository.DeleteByIdAsync(
                parent.Id,
                CancellationToken.None);

            // Assert
            Assert.True(deleted);
            Assert.False(await _context.Comments.AnyAsync(c => c.Id == parent.Id));
            Assert.False(await _context.Comments.AnyAsync(c => c.Id == reply1.Id));
            Assert.False(await _context.Comments.AnyAsync(c => c.Id == reply2.Id));
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenReply_DeletesOnlyReply()
        {
            // Arrange
            var parent = new Comment(
                null,
                "John",
                "john@test.com",
                "Parent",
                null,
                null,
                null);

            await _repository.AddAsync(parent, CancellationToken.None);

            var reply = new Comment(
                parent.Id,
                "Mike",
                "mike@test.com",
                "Reply",
                null,
                null,
                null);

            await _repository.AddAsync(reply, CancellationToken.None);

            // Act
            var deleted = await _repository.DeleteByIdAsync(
                reply.Id,
                CancellationToken.None);

            // Assert
            Assert.True(deleted);
            Assert.False(await _context.Comments.AnyAsync(c => c.Id == reply.Id));
            Assert.True(await _context.Comments.AnyAsync(c => c.Id == parent.Id));
        }

        [Fact]
        public async Task DeleteByIdAsync_WhenCommentDoesNotExist_ReturnsFalse()
        {
            // Act
            var deleted = await _repository.DeleteByIdAsync(
                999999,
                CancellationToken.None);

            // Assert
            Assert.False(deleted);
        }

        [Fact]
        public async Task GetListOffsetAsync_ReturnsOnlyParentComments()
        {
            // Arrange
            var parent = new Comment(
                null,
                "John",
                "john@test.com",
                "Parent",
                null,
                null,
                null);

            await _repository.AddAsync(parent, CancellationToken.None);

            var reply = new Comment(
                parent.Id,
                "Mike",
                "mike@test.com",
                "Reply",
                null,
                null,
                null);

            await _repository.AddAsync(reply, CancellationToken.None);

            var query = new CommentQuery
            {
                SortBy = CommentSortField.userName,
                Ascending = false,
            };

            // Act
            var result = await _repository.GetListOffsetAsync(
                query,
                CancellationToken.None);

            // Assert
            Assert.True(await _context.Comments.AnyAsync(c => c.Id == parent.Id));
        }
    }
}
