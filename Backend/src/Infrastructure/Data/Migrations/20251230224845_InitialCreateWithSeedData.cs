using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Comments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateWithSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TextFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OriginalTextFileName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "CreatedAt", "Email", "ImageId", "OriginalTextFileName", "ParentId", "Text", "TextFileId", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 28, 23, 29, 0, 400, DateTimeKind.Unspecified), "www@gmail.com", null, null, null, "This is a great post!", null, "Alice" },
                    { 2, new DateTime(2025, 12, 29, 10, 10, 0, 0, DateTimeKind.Unspecified), "bob@mail.com", null, null, null, "I really enjoyed reading this.", null, "Bob" },
                    { 3, new DateTime(2025, 12, 29, 10, 15, 0, 0, DateTimeKind.Unspecified), "charlie@mail.com", null, null, null, "Very informative, thanks!", null, "Charlie" },
                    { 4, new DateTime(2025, 12, 29, 10, 20, 0, 0, DateTimeKind.Unspecified), "diana@mail.com", null, null, null, "Could you share more details?", null, "Diana" },
                    { 5, new DateTime(2025, 12, 29, 10, 25, 0, 0, DateTimeKind.Unspecified), "edward@mail.com", null, null, null, "This helped me a lot.", null, "Edward" },
                    { 6, new DateTime(2025, 12, 29, 10, 30, 0, 0, DateTimeKind.Unspecified), "fiona@mail.com", null, null, null, "Clear and well written.", null, "Fiona" },
                    { 7, new DateTime(2025, 12, 29, 10, 35, 0, 0, DateTimeKind.Unspecified), "george@mail.com", null, null, null, "I have a different opinion.", null, "George" },
                    { 8, new DateTime(2025, 12, 29, 10, 40, 0, 0, DateTimeKind.Unspecified), "helen@mail.com", null, null, null, "Nice explanation!", null, "Helen" },
                    { 9, new DateTime(2025, 12, 29, 10, 45, 0, 0, DateTimeKind.Unspecified), "ian@mail.com", null, null, null, "Looking forward to the next post.", null, "Ian" },
                    { 10, new DateTime(2025, 12, 29, 10, 50, 0, 0, DateTimeKind.Unspecified), "julia@mail.com", null, null, null, "This topic is very relevant.", null, "Julia" },
                    { 11, new DateTime(2025, 12, 29, 10, 55, 0, 0, DateTimeKind.Unspecified), "kevin@mail.com", null, null, null, "Thanks for sharing!", null, "Kevin" },
                    { 12, new DateTime(2025, 12, 29, 11, 0, 0, 0, DateTimeKind.Unspecified), "laura@mail.com", null, null, null, "Good overview of the problem.", null, "Laura" },
                    { 13, new DateTime(2025, 12, 29, 11, 5, 0, 0, DateTimeKind.Unspecified), "michael@mail.com", null, null, null, "I learned something new today.", null, "Michael" },
                    { 14, new DateTime(2025, 12, 29, 11, 10, 0, 0, DateTimeKind.Unspecified), "nina@mail.com", null, null, null, "Well structured and easy to read.", null, "Nina" },
                    { 15, new DateTime(2025, 12, 29, 11, 15, 0, 0, DateTimeKind.Unspecified), "oscar@mail.com", null, null, null, "Can you provide examples?", null, "Oscar" },
                    { 16, new DateTime(2025, 12, 29, 11, 20, 0, 0, DateTimeKind.Unspecified), "paul@mail.com", null, null, null, "This answered my question.", null, "Paul" },
                    { 17, new DateTime(2025, 12, 29, 11, 25, 0, 0, DateTimeKind.Unspecified), "andrey@mail.ru", null, null, null, "Отличная статья, спасибо!", null, "Андрей" },
                    { 18, new DateTime(2025, 12, 29, 11, 30, 0, 0, DateTimeKind.Unspecified), "maria@mail.ru", null, null, null, "Очень полезная информация.", null, "Мария" },
                    { 19, new DateTime(2025, 12, 29, 11, 35, 0, 0, DateTimeKind.Unspecified), "ivan@mail.ru", null, null, null, "Полностью согласен с автором.", null, "Иван" },
                    { 20, new DateTime(2025, 12, 29, 11, 40, 0, 0, DateTimeKind.Unspecified), "olga@mail.ru", null, null, null, "Хорошо объяснено и понятно.", null, "Ольга" },
                    { 21, new DateTime(2025, 12, 29, 11, 45, 0, 0, DateTimeKind.Unspecified), "sergey@mail.ru", null, null, null, "Интересная тема для обсуждения.", null, "Сергей" },
                    { 22, new DateTime(2025, 12, 29, 11, 50, 0, 0, DateTimeKind.Unspecified), "kate@mail.ru", null, null, null, "Спасибо за подробное описание.", null, "Екатерина" },
                    { 23, new DateTime(2025, 12, 29, 11, 55, 0, 0, DateTimeKind.Unspecified), "dmitry@mail.ru", null, null, null, "Было полезно прочитать.", null, "Дмитрий" },
                    { 24, new DateTime(2025, 12, 29, 12, 0, 0, 0, DateTimeKind.Unspecified), "nataly@mail.ru", null, null, null, "Жду продолжения.", null, "Наталья" },
                    { 25, new DateTime(2025, 12, 29, 12, 5, 0, 0, DateTimeKind.Unspecified), "alex@mail.ru", null, null, null, "Хороший пример реализации.", null, "Алексей" },
                    { 26, new DateTime(2025, 12, 29, 12, 10, 0, 0, DateTimeKind.Unspecified), "irina@mail.ru", null, null, null, "Все разложено по полочкам.", null, "Ирина" },
                    { 27, new DateTime(2025, 12, 29, 12, 15, 0, 0, DateTimeKind.Unspecified), "pavel@mail.ru", null, null, null, "Информация актуальна.", null, "Павел" },
                    { 28, new DateTime(2025, 12, 29, 12, 20, 0, 0, DateTimeKind.Unspecified), "tatiana@mail.ru", null, null, null, "Спасибо, было интересно.", null, "Татьяна" },
                    { 29, new DateTime(2025, 12, 29, 12, 25, 0, 0, DateTimeKind.Unspecified), "victor@mail.ru", null, null, null, "Хорошая подача материала.", null, "Виктор" },
                    { 30, new DateTime(2025, 12, 29, 12, 30, 0, 0, DateTimeKind.Unspecified), "elena@mail.ru", null, null, null, "Полезно для начинающих.", null, "Елена" },
                    { 31, new DateTime(2025, 12, 30, 23, 29, 0, 0, DateTimeKind.Unspecified), "www@gmail.com", null, null, 1, "This topic is very relevant.", null, "Alice" },
                    { 32, new DateTime(2025, 12, 30, 10, 10, 0, 0, DateTimeKind.Unspecified), "bob@mail.com", null, null, 1, "I really enjoyed reading this.", null, "Bob" },
                    { 33, new DateTime(2025, 12, 30, 10, 15, 0, 0, DateTimeKind.Unspecified), "charlie@mail.com", null, null, 1, "Very informative, thanks!", null, "Charlie" },
                    { 34, new DateTime(2025, 12, 30, 10, 20, 0, 0, DateTimeKind.Unspecified), "diana@mail.com", null, null, 1, "Could you share more details?", null, "Diana" },
                    { 35, new DateTime(2025, 12, 30, 10, 25, 0, 0, DateTimeKind.Unspecified), "edward@mail.com", null, null, 1, "This helped me a lot.", null, "Edward" },
                    { 36, new DateTime(2025, 12, 30, 10, 30, 0, 0, DateTimeKind.Unspecified), "fiona@mail.com", null, null, 1, "Clear and well written.", null, "Fiona" },
                    { 37, new DateTime(2025, 12, 30, 10, 35, 0, 0, DateTimeKind.Unspecified), "george@mail.com", null, null, 1, "I have a different opinion.", null, "George" },
                    { 38, new DateTime(2025, 12, 30, 10, 40, 0, 0, DateTimeKind.Unspecified), "helen@mail.com", null, null, 1, "Nice explanation!", null, "Helen" },
                    { 39, new DateTime(2025, 12, 30, 10, 45, 0, 0, DateTimeKind.Unspecified), "ian@mail.com", null, null, 1, "Looking forward to the next post.", null, "Ian" },
                    { 40, new DateTime(2025, 12, 30, 10, 50, 0, 0, DateTimeKind.Unspecified), "julia@mail.com", null, null, 1, "This topic is very relevant.", null, "Julia" },
                    { 41, new DateTime(2025, 12, 30, 11, 45, 0, 0, DateTimeKind.Unspecified), "sergey@mail.ru", null, null, 2, "Интересная тема для обсуждения.", null, "Сергей" },
                    { 42, new DateTime(2025, 12, 30, 11, 50, 0, 0, DateTimeKind.Unspecified), "kate@mail.ru", null, null, 2, "Спасибо за подробное описание.", null, "Екатерина" },
                    { 43, new DateTime(2025, 12, 30, 11, 55, 0, 0, DateTimeKind.Unspecified), "dmitry@mail.ru", null, null, 2, "Было полезно прочитать.", null, "Дмитрий" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentId",
                table: "Comments",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");
        }
    }
}
