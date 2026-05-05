using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Comments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexOnCommentsParentIdCreatedAtId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentId_CreatedAt",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId_CreatedAt_Id",
                table: "Comments",
                columns: new[] { "ParentId", "CreatedAt", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Comments_ParentId_CreatedAt_Id",
                table: "Comments");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId_CreatedAt",
                table: "Comments",
                columns: new[] { "ParentId", "CreatedAt" });
        }
    }
}
