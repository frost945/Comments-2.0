using Comments.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace tests.IntegrationTests.Fixtures
{
    public class SqlServerFixture : IDisposable
    {
        public CommentsDbContext Context { get; }

        public SqlServerFixture()
        {
            var options = new DbContextOptionsBuilder<CommentsDbContext>()
                .UseSqlServer(
                    @"Server=localhost;
                  Database=Comments_Test;
                  Trusted_Connection=True;
                  TrustServerCertificate=True")
                .Options;

            Context = new CommentsDbContext(options);

            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}
