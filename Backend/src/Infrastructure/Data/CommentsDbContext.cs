using Microsoft.EntityFrameworkCore;
using Comments.Models;
using Comments.Infrastructure.Configurations;

namespace Comments.Infrastructure.Data
{
    public class CommentsDbContext : DbContext
    {
        public DbSet<Comment> Comments { get; set; }

        public CommentsDbContext(DbContextOptions<CommentsDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
        }
    }
}
