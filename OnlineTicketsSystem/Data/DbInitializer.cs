using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineTicketsSystem.Models;
using OnlineTicketsSystem.Data.Seed;
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
            //if (!context.Categories.Any())
            //{
            //    var categories = new Category[]
            //    {
            //        new Category { Name = "Театър" },
            //        new Category { Name = "Концерт" },
            //        new Category { Name = "Спорт" },
            //        new Category { Name = "Кино" },
            //         new Category { Name = "Фестивал" },
            //         new Category { Name =  "Комедия" },
            //         new Category { Name = "Опера и балет"},
            //         new Category { Name = "Детски събития" },
            //           new Category { Name =  "Стендъп" },
            //             new Category { Name =     "Изложба" },



            //    };
            //    context.Categories.AddRange(categories);
            //    await context.SaveChangesAsync();
            //}
            await CategorySeeder.SeedAsync(context);

           
            await EventSeeder.SeedAsync(context);
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