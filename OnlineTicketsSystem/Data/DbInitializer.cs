using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineTicketsSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;



namespace OnlineTicketsSystem.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                // --- Градове (Cities) – диагностика + добавя липсващите от файла ---
                try
                {
                    var env = services.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();

                    // Път 1: проектната папка (ContentRootPath)
                    var path1 = Path.Combine(env.ContentRootPath, "Data", "Seed", "cities-bg.txt");

                    // Път 2: output/bin папката (ако VS стартира оттам)
                    var path2 = Path.Combine(AppContext.BaseDirectory, "Data", "Seed", "cities-bg.txt");

                    string? filePath = null;

                    if (File.Exists(path1)) filePath = path1;
                    else if (File.Exists(path2)) filePath = path2;

                    Console.WriteLine($"[Cities seed] ContentRootPath: {env.ContentRootPath}");
                    Console.WriteLine($"[Cities seed] BaseDirectory: {AppContext.BaseDirectory}");
                    Console.WriteLine($"[Cities seed] path1 exists: {File.Exists(path1)} -> {path1}");
                    Console.WriteLine($"[Cities seed] path2 exists: {File.Exists(path2)} -> {path2}");

                    if (filePath == null)
                    {
                        Console.WriteLine("[Cities seed] ERROR: cities-bg.txt NOT FOUND.");
                    }
                    else
                    {
                        var lines = File.ReadAllLines(filePath);
                        Console.WriteLine($"[Cities seed] Using file: {filePath}");
                        Console.WriteLine($"[Cities seed] Lines in file: {lines.Length}");

                        int added = 0;

                        foreach (var line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var parts = line.Split('|', StringSplitOptions.TrimEntries);
                            if (parts.Length != 2) continue;

                            var name = parts[0];
                            var slug = parts[1];

                            if (context.Cities.Any(c => c.Slug == slug))
                                continue;

                            context.Cities.Add(new City { Name = name, Slug = slug });
                            added++;
                        }

                        context.SaveChanges();
                        Console.WriteLine($"[Cities seed] Added cities: {added}");
                        Console.WriteLine($"[Cities seed] Total cities in DB now: {context.Cities.Count()}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Cities seed] EXCEPTION: " + ex);
                }

                //var env = services.GetRequiredService<IWebHostEnvironment>();
                //var filePath = Path.Combine(env.ContentRootPath, "Data", "Seed", "cities-bg.txt");

                /* if (File.Exists(filePath))
                 {
                     var lines = File.ReadAllLines(filePath);

                     foreach (var line in lines)
                     {
                         if (string.IsNullOrWhiteSpace(line)) continue;

                         var parts = line.Split('|', StringSplitOptions.TrimEntries);
                         if (parts.Length != 2) continue;

                         var name = parts[0];
                         var slug = parts[1];

                         // проверка дали вече го има
                         if (context.Cities.Any(c => c.Slug == slug))
                             continue;

                         context.Cities.Add(new City
                         {
                             Name = name,
                             Slug = slug
                         });
                     }

                     context.SaveChanges();
                 }
 */



                // --- Категории ---
                if (!context.Categories.Any())
                {
                    var categories = new Category[]
                    {
                        new Category { Name = "Театър" },
                        new Category { Name = "Концерт" },
                        new Category { Name = "Спорт" },
                        new Category { Name = "Кино" }
                    };
                    context.Categories.AddRange(categories);
                    context.SaveChanges();
                }

                //// --- Събития ---
                //if (!context.Events.Any())
                //{
                //    var events = new Event[]
                //    {
                //        new Event
                //        {
                //            Title = "Концерт на популярна група",
                //            Description = "Музика на живо в Разград",
                //            City = "Разград",
                //            Venue = "Общински културен център",
                //            Date = DateTime.Now.AddDays(10),
                //            Capacity = 100,
                //            ImageUrl = "",
                //            CategoryId = context.Categories.First(c => c.Name == "Концерт").Id
                //        },
                //        new Event
                //        {
                //            Title = "Театрална постановка",
                //            Description = "Драма в Пловдив",
                //            City = "Пловдив",
                //            Venue = "Античен театър",
                //            Date = DateTime.Now.AddDays(15),
                //            Capacity = 50,
                //            ImageUrl = "",
                //            CategoryId = context.Categories.First(c => c.Name == "Театър").Id
                //        }
                //    };
                //    context.Events.AddRange(events);
                //    context.SaveChanges();
                //}


            // --- Събития ---
            var eventsToSeed = new List<Event>
{
    new Event
    {
        Title = "Рок концерт на открито",
        Description = "Голям рок концерт в центъра на София",
        City = "София",
        Venue = "Арена София",
        Date = DateTime.Now.AddDays(20),
        Capacity = 500,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Концерт").Id
    },
    new Event
    {
        Title = "Футболен мач",
        Description = "Дерби мач от първа лига",
        City = "Пловдив",
        Venue = "Стадион Пловдив",
        Date = DateTime.Now.AddDays(12),
        Capacity = 800,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Спорт").Id
    },
    new Event
    {
        Title = "Театрална комедия",
        Description = "Комедийна постановка",
        City = "Варна",
        Venue = "Драматичен театър",
        Date = DateTime.Now.AddDays(18),
        Capacity = 200,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Театър").Id
    },
    new Event
    {
        Title = "Кино премиера",
        Description = "Премиера на нов български филм",
        City = "Бургас",
        Venue = "Cinema City",
        Date = DateTime.Now.AddDays(7),
        Capacity = 150,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Кино").Id
    },
    new Event
    {
        Title = "Джаз вечер",
        Description = "Джаз концерт на живо",
        City = "Русе",
        Venue = "Културен дом",
        Date = DateTime.Now.AddDays(25),
        Capacity = 120,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Концерт").Id
    },
    new Event
    {
        Title = "Балетно представление",
        Description = "Класически балет",
        City = "Стара Загора",
        Venue = "Опера Стара Загора",
        Date = DateTime.Now.AddDays(30),
        Capacity = 300,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Театър").Id
    },
    new Event
    {
        Title = "Баскетболен турнир",
        Description = "Регионален турнир",
        City = "Плевен",
        Venue = "Спортна зала Плевен",
        Date = DateTime.Now.AddDays(14),
        Capacity = 400,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Спорт").Id
    },
    new Event
    {
        Title = "Фестивал на изкуствата",
        Description = "Изложби и музика",
        City = "Велико Търново",
        Venue = "Стария град",
        Date = DateTime.Now.AddDays(40),
        Capacity = 600,
        ImageUrl = "",
        CategoryId = context.Categories.First(c => c.Name == "Концерт").Id
    }
};

            // добавяме само тези, които ги няма
            foreach (var ev in eventsToSeed)
            {
                if (!context.Events.Any(e => e.Title == ev.Title))
                {
                    context.Events.Add(ev);
                }
            }

            context.SaveChanges();

                // --- Роли ---
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                // --- Админ потребител ---
                var adminEmail = "admin@tickets.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var createAdmin = await userManager.CreateAsync(adminUser, "Admin123!");
                    if (!createAdmin.Succeeded)
                    {
                        foreach (var error in createAdmin.Errors)
                            Console.WriteLine($"Admin create error: {error.Description}");
                    }
                }

                // Даваме Admin роля
                if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }



                // --- Тестов потребител ---
                var existingUser = userManager.FindByEmailAsync("testuser@example.com").Result;
                IdentityUser testUser;
                if (existingUser == null)
                {
                    testUser = new IdentityUser
                    {
                        UserName = "test@example.com",
                        Email = "test@example.com",
                        EmailConfirmed = true
                    };

                    var result = userManager.CreateAsync(testUser, "tesT123!").Result;

                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error creating user: {error.Description}");
                        }
                    }
                }
                else
                {
                    testUser = existingUser;
                }

                // --- Тестови билети ---
                if (!context.Tickets.Any())
                {
                    var firstEvent = context.Events.First();

                    var tickets = new Ticket[]
                    {
                        new Ticket
                        {
                            EventId = firstEvent.Id,
                            UserId = testUser.Id,
                            PurchaseDate = DateTime.Now
                        },
                        new Ticket
                        {
                            EventId = firstEvent.Id,
                            UserId = testUser.Id,
                            PurchaseDate = DateTime.Now
                        }
                    };

                    context.Tickets.AddRange(tickets);
                    context.SaveChanges();
                }
            }
        }

    }
}





