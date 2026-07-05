using Comments.Domain.Models;
using Comments.Application.Interfaces.Repositories;
using Comments.Application.Requests;
using Comments.Application.Queries.Enums;
using Comments.Application.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Comments.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentsDbContext _dbContext;

        public CommentRepository(CommentsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Comment comment, CancellationToken ct)
        {
            await _dbContext.Comments.AddAsync(comment, ct);
            await _dbContext.SaveChangesAsync(ct);
        }

        public async Task<bool> ParentExistsAsync(int parentId, CancellationToken ct)
        {
            return await _dbContext.Comments.AnyAsync(c => c.Id == parentId, ct);
        }

        public async Task<CommentDto?> GetByIdAsync(int id, CancellationToken ct)
        {
           return await _dbContext.Comments
                .Where(c => c.Id == id)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageId = c.ImageId,
                    TextFileId = c.TextFileId,
                    TextFileName = c.OriginalTextFileName,
                    ReplyCount = c.Children.Count
                })
                .FirstOrDefaultAsync(ct);
        }

        // sorting by UserName, Email
        public async Task<List<CommentDto>> GetListOffsetAsync(CommentQuery commentQuery, CancellationToken ct, int? parentId = null)
        {
            var comments = _dbContext.Comments
                .AsNoTracking()
                .AsQueryable();

            comments = parentId == null
                ? comments.Where(c => c.ParentId == null)
                : comments.Where(c => c.ParentId == parentId);

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

            var commentsDto = await comments
                .Skip(commentQuery.Skip)
                .Take(commentQuery.PageSize)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageId = c.ImageId,
                    TextFileId = c.TextFileId,
                    TextFileName = c.OriginalTextFileName,
                    ReplyCount = parentId == null
                    ? c.Children.Count
                    : 0 // count the number of replies only for parent comments
                })
                .ToListAsync(ct);

            return commentsDto;
        }

        // keyset pagination implementation only for sorting by createdAt field
        public async Task<List<CommentDto>> GetListKeysetAsync(CommentQuery commentQuery, CancellationToken ct, int? parentId = null)
        {
            var comments = _dbContext.Comments
                .AsNoTracking()
                .AsQueryable();

            comments = parentId == null
                ? comments.Where(c => c.ParentId == null)
                : comments.Where(c => c.ParentId == parentId);

            if (commentQuery.CursorCreatedAt != null && commentQuery.CursorId != null)
            {
                //move next page
                if (commentQuery.Direction == true)
                {
                    if (commentQuery.Ascending)
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

                        comments = comments.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id);
                    }
                }
                //move previous page
                else
                {
                    if (commentQuery.Ascending)
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
                Console.WriteLine("First page or no cursor provided, using default sorting");
                comments = commentQuery.Ascending
                    ? comments.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id)
                    : comments.OrderByDescending(c => c.CreatedAt).ThenByDescending(c => c.Id);
            }

            var commentsDto = await comments
                .Take(commentQuery.PageSize)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserName = c.UserName,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt,
                    ImageId = c.ImageId,
                    TextFileId = c.TextFileId,
                    TextFileName = c.OriginalTextFileName,
                    ReplyCount = parentId == null
                    ? c.Children.Count
                    : 0 // count the number of replies only for parent comments
                })
                .ToListAsync(ct);

            return commentsDto;
        }

        public async Task<bool> DeleteByIdAsync(int id, CancellationToken ct)
        {
            var isParent = await _dbContext.Comments.AnyAsync(c => c.Id == id && c.ParentId == null, ct);

            if (isParent)
            {   //cascade delete all replies of the parent comment
                await _dbContext.Comments.Where(c => c.ParentId == id)
                .ExecuteDeleteAsync(ct);

                return await _dbContext.Comments
                    .Where(c => c.Id == id)
                    .ExecuteDeleteAsync(ct) > 0;
            }
            else
            {   //delete only the reply
                return await _dbContext.Comments
                .Where(c => c.Id == id && c.ParentId != null)
                .ExecuteDeleteAsync(ct) > 0;
            }
        }
    }   
}
