using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnlineTicketsSystem.Models;


namespace OnlineTicketsSystem.Data
{
    public class DbInitializer
    {
        public static void Seed(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

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

                // --- Събития ---
                if (!context.Events.Any())
                {
                    var events = new Event[]
                    {
                        new Event
                        {
                            Title = "Концерт на популярна група",
                            Description = "Музика на живо в Разград",
                            City = "Разград",
                            Venue = "Общински културен център",
                            Date = DateTime.Now.AddDays(10),
                            Capacity = 100,
                            ImageUrl = "",
                            CategoryId = context.Categories.First(c => c.Name == "Концерт").Id
                        },
                        new Event
                        {
                            Title = "Театрална постановка",
                            Description = "Драма в Пловдив",
                            City = "Пловдив",
                            Venue = "Античен театър",
                            Date = DateTime.Now.AddDays(15),
                            Capacity = 50,
                            ImageUrl = "",
                            CategoryId = context.Categories.First(c => c.Name == "Театър").Id
                        }
                    };
                    context.Events.AddRange(events);
                    context.SaveChanges();
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





