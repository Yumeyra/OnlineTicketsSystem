using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;

namespace OnlineTicketsSystem.Data.Seed
{
    public class CategorySeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var categoryNames = new[]
            {
                "Театър",
                "Концерт",
                "Спорт",
                "Кино",
                "Фестивал",
                "Комедия",
                "Опера и балет",
                "Детски събития",
                "Стендъп",
                "Изложба"
            };

            var existingNames = await context.Categories
                .IgnoreQueryFilters()
                .Select(c => c.Name)
                .ToListAsync();

            var missingCategories = categoryNames
                .Where(name => !existingNames.Contains(name))
                .Select(name => new Category
                {
                    Name = name,
                    IsDeleted = false,
                    DeletedAt = null
                })
                .ToList();

            if (missingCategories.Any())
            {
                context.Categories.AddRange(missingCategories);
                await context.SaveChangesAsync();
            }
        }
    }
}
