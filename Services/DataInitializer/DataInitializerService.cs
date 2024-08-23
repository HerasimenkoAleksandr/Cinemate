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
               Name = "Romantic Comedy",
               Description = "This subcategory features romantic comedies, blending humor with romantic storylines. Films in this category typically involve charming love stories, often with light-hearted and humorous twists.",
               ParentCategoryId = anyCategory.Id,
               Picture = "https://afisha.bigmir.net/i/72/21/12/8/7221128/image_main/ad60259f8762d95d84de6e641186bdc3-quality_75Xresize_crop_1Xallow_enlarge_0Xw_790Xh_445.jpg",
               ContentCount = 0
            },
            new SubCategoriesEntity
            {
               Name = "Classic Comedy",
               Description = "This subcategory includes classic comedies known for their timeless humor and influential comedic styles. Films here often feature iconic comedic moments and legendary performances that have stood the test of time.",
               ParentCategoryId = anyCategory.Id,
               Picture = "https://www.mkin24.ru/_ld/20/34593735.jpg",
               ContentCount = 0
            },
            new SubCategoriesEntity
            {
                Name = "Satirical Comedy",
                Description = "This subcategory focuses on satirical comedies that use humor, irony, and exaggeration to critique and comment on societal issues, politics, and cultural norms. These films often offer a sharp and clever perspective on contemporary topics.",
                ParentCategoryId = anyCategory.Id,
                Picture = "https://opis-cdn.tinkoffjournal.ru/mercury/best-comedies-ever_16.qwvobcft3lky..jpg",
                ContentCount = 0
}
            };

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
