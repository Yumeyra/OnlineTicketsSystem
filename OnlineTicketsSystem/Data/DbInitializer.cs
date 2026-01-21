using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineTicketsSystem.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;



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





