using Comments.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Comments.Infrastructure.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
               .HasOne(c => c.Parent)
               .WithMany(c => c.Children)
               .HasForeignKey(c => c.ParentId)
               .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            builder.HasData(
                new { Id = 1, ParentId = (int?)null, UserName = "Alice", Email = "www@gmail.com", Text = "This is a great post!", CreatedAt = new DateTime(2025, 12, 28, 23, 29, 00, 400) },
                new { Id = 2, ParentId = (int?)null, UserName = "Bob", Email = "bob@mail.com", Text = "I really enjoyed reading this.", CreatedAt = new DateTime(2025, 12, 29, 10, 10, 00) },
                new { Id = 3, ParentId = (int?)null, UserName = "Charlie", Email = "charlie@mail.com", Text = "Very informative, thanks!", CreatedAt = new DateTime(2025, 12, 29, 10, 15, 00) },
                new { Id = 4, ParentId = (int?)null, UserName = "Diana", Email = "diana@mail.com", Text = "Could you share more details?", CreatedAt = new DateTime(2025, 12, 29, 10, 20, 00) },
                new { Id = 5, ParentId = (int?)null, UserName = "Edward", Email = "edward@mail.com", Text = "This helped me a lot.", CreatedAt = new DateTime(2025, 12, 29, 10, 25, 00) },
                new { Id = 6, ParentId = (int?)null, UserName = "Fiona", Email = "fiona@mail.com", Text = "Clear and well written.", CreatedAt = new DateTime(2025, 12, 29, 10, 30, 00) },
                new { Id = 7, ParentId = (int?)null, UserName = "George", Email = "george@mail.com", Text = "I have a different opinion.", CreatedAt = new DateTime(2025, 12, 29, 10, 35, 00) },
                new { Id = 8, ParentId = (int?)null, UserName = "Helen", Email = "helen@mail.com", Text = "Nice explanation!", CreatedAt = new DateTime(2025, 12, 29, 10, 40, 00) },
                new { Id = 9, ParentId = (int?)null, UserName = "Ian", Email = "ian@mail.com", Text = "Looking forward to the next post.", CreatedAt = new DateTime(2025, 12, 29, 10, 45, 00) },
                new { Id = 10, ParentId = (int?)null, UserName = "Julia", Email = "julia@mail.com", Text = "This topic is very relevant.", CreatedAt = new DateTime(2025, 12, 29, 10, 50, 00) },
                new { Id = 11, ParentId = (int?)null, UserName = "Kevin", Email = "kevin@mail.com", Text = "Thanks for sharing!", CreatedAt = new DateTime(2025, 12, 29, 10, 55, 00) },
                new { Id = 12, ParentId = (int?)null, UserName = "Laura", Email = "laura@mail.com", Text = "Good overview of the problem.", CreatedAt = new DateTime(2025, 12, 29, 11, 00, 00) },
                new { Id = 13, ParentId = (int?)null, UserName = "Michael", Email = "michael@mail.com", Text = "I learned something new today.", CreatedAt = new DateTime(2025, 12, 29, 11, 05, 00) },
                new { Id = 14, ParentId = (int?)null, UserName = "Nina", Email = "nina@mail.com", Text = "Well structured and easy to read.", CreatedAt = new DateTime(2025, 12, 29, 11, 10, 00) },
                new { Id = 15, ParentId = (int?)null, UserName = "Oscar", Email = "oscar@mail.com", Text = "Can you provide examples?", CreatedAt = new DateTime(2025, 12, 29, 11, 15, 00) },
                new { Id = 16, ParentId = (int?)null, UserName = "Paul", Email = "paul@mail.com", Text = "This answered my question.", CreatedAt = new DateTime(2025, 12, 29, 11, 20, 00) },
                new { Id = 17, ParentId = (int?)null, UserName = "Андрей", Email = "andrey@mail.ru", Text = "Отличная статья, спасибо!", CreatedAt = new DateTime(2025, 12, 29, 11, 25, 00) },
                new { Id = 18, ParentId = (int?)null, UserName = "Мария", Email = "maria@mail.ru", Text = "Очень полезная информация.", CreatedAt = new DateTime(2025, 12, 29, 11, 30, 00) },
                new { Id = 19, ParentId = (int?)null, UserName = "Иван", Email = "ivan@mail.ru", Text = "Полностью согласен с автором.", CreatedAt = new DateTime(2025, 12, 29, 11, 35, 00) },
                new { Id = 20, ParentId = (int?)null, UserName = "Ольга", Email = "olga@mail.ru", Text = "Хорошо объяснено и понятно.", CreatedAt = new DateTime(2025, 12, 29, 11, 40, 00) },
                new { Id = 21, ParentId = (int?)null, UserName = "Сергей", Email = "sergey@mail.ru", Text = "Интересная тема для обсуждения.", CreatedAt = new DateTime(2025, 12, 29, 11, 45, 00) },
                new { Id = 22, ParentId = (int?)null, UserName = "Екатерина", Email = "kate@mail.ru", Text = "Спасибо за подробное описание.", CreatedAt = new DateTime(2025, 12, 29, 11, 50, 00) },
                new { Id = 23, ParentId = (int?)null, UserName = "Дмитрий", Email = "dmitry@mail.ru", Text = "Было полезно прочитать.", CreatedAt = new DateTime(2025, 12, 29, 11, 55, 00) },
                new { Id = 24, ParentId = (int?)null, UserName = "Наталья", Email = "nataly@mail.ru", Text = "Жду продолжения.", CreatedAt = new DateTime(2025, 12, 29, 12, 00, 00) },
                new { Id = 25, ParentId = (int?)null, UserName = "Алексей", Email = "alex@mail.ru", Text = "Хороший пример реализации.", CreatedAt = new DateTime(2025, 12, 29, 12, 05, 00) },
                new { Id = 26, ParentId = (int?)null, UserName = "Ирина", Email = "irina@mail.ru", Text = "Все разложено по полочкам.", CreatedAt = new DateTime(2025, 12, 29, 12, 10, 00) },
                new { Id = 27, ParentId = (int?)null, UserName = "Павел", Email = "pavel@mail.ru", Text = "Информация актуальна.", CreatedAt = new DateTime(2025, 12, 29, 12, 15, 00) },
                new { Id = 28, ParentId = (int?)null, UserName = "Татьяна", Email = "tatiana@mail.ru", Text = "Спасибо, было интересно.", CreatedAt = new DateTime(2025, 12, 29, 12, 20, 00) },
                new { Id = 29, ParentId = (int?)null, UserName = "Виктор", Email = "victor@mail.ru", Text = "Хорошая подача материала.", CreatedAt = new DateTime(2025, 12, 29, 12, 25, 00) },
                new { Id = 30, ParentId = (int?)null, UserName = "Елена", Email = "elena@mail.ru", Text = "Полезно для начинающих.", CreatedAt = new DateTime(2025, 12, 29, 12, 30, 00) },

                new { Id = 31, ParentId = 1, UserName = "Alice", Email = "www@gmail.com", Text = "This topic is very relevant.", CreatedAt = new DateTime(2025, 12, 30, 23, 29, 00) },
                new { Id = 32, ParentId = 1, UserName = "Bob", Email = "bob@mail.com", Text = "I really enjoyed reading this.", CreatedAt = new DateTime(2025, 12, 30, 10, 10, 00) },
                new { Id = 33, ParentId = 1, UserName = "Charlie", Email = "charlie@mail.com", Text = "Very informative, thanks!", CreatedAt = new DateTime(2025, 12, 30, 10, 15, 00) },
                new { Id = 34, ParentId = 1, UserName = "Diana", Email = "diana@mail.com", Text = "Could you share more details?", CreatedAt = new DateTime(2025, 12, 30, 10, 20, 00) },
                new { Id = 35, ParentId = 1, UserName = "Edward", Email = "edward@mail.com", Text = "This helped me a lot.", CreatedAt = new DateTime(2025, 12, 30, 10, 25, 00) },
                new { Id = 36, ParentId = 1, UserName = "Fiona", Email = "fiona@mail.com", Text = "Clear and well written.", CreatedAt = new DateTime(2025, 12, 30, 10, 30, 00) },
                new { Id = 37, ParentId = 1, UserName = "George", Email = "george@mail.com", Text = "I have a different opinion.", CreatedAt = new DateTime(2025, 12, 30, 10, 35, 00) },
                new { Id = 38, ParentId = 1, UserName = "Helen", Email = "helen@mail.com", Text = "Nice explanation!", CreatedAt = new DateTime(2025, 12, 30, 10, 40, 00) },
                new { Id = 39, ParentId = 1, UserName = "Ian", Email = "ian@mail.com", Text = "Looking forward to the next post.", CreatedAt = new DateTime(2025, 12, 30, 10, 45, 00) },
                new { Id = 40, ParentId = 1, UserName = "Julia", Email = "julia@mail.com", Text = "This topic is very relevant.", CreatedAt = new DateTime(2025, 12, 30, 10, 50, 00) },

                new { Id = 41, ParentId = 2, UserName = "Сергей", Email = "sergey@mail.ru", Text = "Интересная тема для обсуждения.", CreatedAt = new DateTime(2025, 12, 30, 11, 45, 00) },
                new { Id = 42, ParentId = 2, UserName = "Екатерина", Email = "kate@mail.ru", Text = "Спасибо за подробное описание.", CreatedAt = new DateTime(2025, 12, 30, 11, 50, 00) },
                new { Id = 43, ParentId = 2, UserName = "Дмитрий", Email = "dmitry@mail.ru", Text = "Было полезно прочитать.", CreatedAt = new DateTime(2025, 12, 30, 11, 55, 00) }
                );
        }
    }
}
