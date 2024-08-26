using cinemate.Data;
using cinemate.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace cinemate.Services.DataInitializer
{
    public class DataInitializerService
    {
        public static async Task InitializeAsync(DataContext context, Guid IdCategory)
        {
            await AddSubCategoriesToCategory(context, IdCategory);
        }

        public static async Task AddSubCategoriesToCategory(DataContext context, Guid IdCategory)
        {
            // 1. Найти категорию "Комедия" по id
            var anyCategory = await context.Gategories
                .FirstOrDefaultAsync(c => c.Id == IdCategory);

            if (anyCategory == null)
            {
                throw new Exception("Категория не найдена.");
            }

            // 2. Подготовить подкатегории для добавления
            var newSubCategories = new List<SubCategoriesEntity>
            {
              new SubCategoriesEntity
            {
               Name = "Anime",
               Description = "A style of animation that originated in Japan and has become popular worldwide. Anime encompasses a wide range of genres and themes, characterized by colorful artwork, vibrant characters, and fantastical storytelling. It includes both TV series and films, often featuring elaborate plots, deep character development, and unique artistic styles. Anime can cover diverse genres such as action, romance, science fiction, fantasy, and horror, appealing to audiences of all ages and interests. The medium often includes cultural elements specific to Japan but has universal themes and narratives that resonate globally.",
               ParentCategoryId = anyCategory.Id,
               Picture = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSk6OCn0t1mJfoNl7rr1zCDzHCN4VO3bd9TFg&s",
               ContentCount = 0
            } };/*,
            new SubCategoriesEntity
            {
               Name = "Classic Detective",
               Description = " Films featuring a classic detective figure, often with a keen intellect and distinctive methods, solving intricate and puzzling crimes. These stories typically follow a structured format with clues, red herrings, and a resolution.\r\n\r\n",
               ParentCategoryId = anyCategory.Id,
               Picture = "https://nbcsports.brightspotcdn.com/dims4/default/81bee85/2147483647/strip/true/crop/638x359+0+1/resize/1440x810!/quality/90/?url=https%3A%2F%2Fnbc-sports-production-nbc-sports.s3.us-east-1.amazonaws.com%2Fbrightspot%2F46%2F92%2F594e4af11a94ca9d4cafe7b71648%2F360070-crop-650x440-e1502765012855.jpeg",
               ContentCount = 0
            } };/* ,
            new SubCategoriesEntity
            {
                Name = "Science and Innovation Biography",
                Description = "Films about scientists, inventors, or innovators, showcasing their discoveries and contributions to science and technology.",
                ParentCategoryId = anyCategory.Id,
                Picture = "https://biographersinternational.org/wp-content/uploads/science.jpg",
                ContentCount = 0
}
            };*/

            // 3. Получить существующие подкатегории в базе данных
            var existingSubCategories = await context.SubCategoriesEntity
                .Where(sc => sc.ParentCategoryId == anyCategory.Id)
                .ToListAsync();

            // 4. Определить подкатегории, которые нужно добавить
            var existingSubCategoryNames = existingSubCategories.Select(sc => sc.Name).ToHashSet();
            var subCategoriesToAdd = newSubCategories
                .Where(sc => !existingSubCategoryNames.Contains(sc.Name))
                .ToList();

            if (subCategoriesToAdd.Any())
            {
                // 5. Добавить только новые подкатегории
                context.SubCategoriesEntity.AddRange(subCategoriesToAdd);
                await context.SaveChangesAsync();
            }
            else
            {
                Console.WriteLine("Все подкатегории уже существуют.");
            }
        }

    }
}
