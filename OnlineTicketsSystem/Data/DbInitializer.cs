using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineTicketsSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineTicketsSystem.Data
{
    public class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var env = services.GetRequiredService<IWebHostEnvironment>();

            // =========================
            // Cities (Name|Slug|Region)
            // =========================
            try
            {
                var path1 = Path.Combine(env.ContentRootPath, "Data", "Seed", "cities-bg.txt");
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
                    var lines = File.ReadAllLines(filePath)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToArray();

                    Console.WriteLine($"[Cities seed] Using file: {filePath}");
                    Console.WriteLine($"[Cities seed] Lines in file: {lines.Length}");

                    int added = 0, updated = 0, skipped = 0;

                    foreach (var rawLine in lines)
                    {
                        var line = rawLine.Trim();
                        if (string.IsNullOrWhiteSpace(line)) { skipped++; continue; }

                        var parts = line.Split('|', StringSplitOptions.TrimEntries);
                        if (parts.Length != 3) { skipped++; continue; }

                        var name = parts[0];
                        var slug = parts[1];
                        var region = parts[2];

                        // търсим по Slug (уникален)
                        var existing = await context.Cities
                            .IgnoreQueryFilters()
                            .FirstOrDefaultAsync(c => c.Slug == slug);

                        if (existing == null)
                        {
                            context.Cities.Add(new City
                            {
                                Name = name,
                                Slug = slug,
                                Region = region,
                                IsDeleted = false,
                                DeletedAt = null
                            });
                            added++;
                        }
                        else
                        {
                            existing.Name = name;
                            existing.Region = region;
                            existing.IsDeleted = false;
                            existing.DeletedAt = null;
                            updated++;
                        }
                    }

                    await context.SaveChangesAsync();

                    Console.WriteLine($"[Cities seed] Added: {added}, Updated: {updated}, Skipped: {skipped}");
                    Console.WriteLine($"[Cities seed] Total cities in DB now: {context.Cities.IgnoreQueryFilters().Count()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Cities seed] EXCEPTION: " + ex);
            }

            // ==========
            // Categories
            // ==========
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
                await context.SaveChangesAsync();
            }

            // ======
            // Events
            // ======
            var eventsToSeed = new List<Event>
            {
                new Event
                {
                    Title = "Рок концерт на открито",
                    Description = "Голям рок концерт в центъра на София",
                    City = "София",
                    Venue = "Арена София",
                    Price = 8.00m,
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
                    Price = 8.90m,
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
                    Price = 10.00m,
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
                    Price = 12.90m,
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
                    Price = 15.00m,
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
                    Price = 16.50m,
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
                    Price = 10.00m,
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
                    Price = 12.90m,
                    Date = DateTime.Now.AddDays(40),
                    Capacity = 600,
                    ImageUrl = "",
                    CategoryId = context.Categories.First(c => c.Name == "Концерт").Id
                }
            };

            foreach (var ev in eventsToSeed)
            {
                var existingEvent = context.Events.FirstOrDefault(e => e.Title == ev.Title);

                if (existingEvent == null)
                {
                    context.Events.Add(ev);
                }
                else
                {
                    existingEvent.Description = ev.Description;
                    existingEvent.City = ev.City;
                    existingEvent.Venue = ev.Venue;
                    existingEvent.Date = ev.Date;
                    existingEvent.Capacity = ev.Capacity;
                    existingEvent.ImageUrl = ev.ImageUrl;
                    existingEvent.CategoryId = ev.CategoryId;
                    existingEvent.Price = ev.Price;
                    existingEvent.IsDeleted = false;
                    existingEvent.DeletedAt = null;
                }
            }

            await context.SaveChangesAsync();

            // =====
            // Roles
            // =====
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // ============
            // Admin user
            // ============
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

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
                await userManager.AddToRoleAsync(adminUser, "Admin");

            // ==============
            // Test user
            // ==============
            var existingUser = await userManager.FindByEmailAsync("test@example.com");
            IdentityUser testUser;

            if (existingUser == null)
            {
                testUser = new IdentityUser
                {
                    UserName = "test@example.com",
                    Email = "test@example.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(testUser, "tesT123!");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                        Console.WriteLine($"Error creating user: {error.Description}");
                }
            }
            else
            {
                testUser = existingUser;
            }

            // ============
            // Test tickets
            // ============
            if (!context.Tickets.Any())
            {
                var firstEvent = context.Events.First();

                var tickets = new Ticket[]
                {
                    new Ticket { EventId = firstEvent.Id, UserId = testUser.Id, PurchaseDate = DateTime.Now },
                    new Ticket { EventId = firstEvent.Id, UserId = testUser.Id, PurchaseDate = DateTime.Now }
                };

                context.Tickets.AddRange(tickets);
                await context.SaveChangesAsync();
            }
        }
    }
}