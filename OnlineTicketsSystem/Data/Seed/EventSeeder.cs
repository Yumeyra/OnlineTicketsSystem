using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace OnlineTicketsSystem.Data.Seed
{
    public class EventSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            //var categoryMap = await context.Categories
            //    .IgnoreQueryFilters()
            //    .ToDictionaryAsync(c => c.Name, c => c);

            //var cityMap = await context.Cities
            //    .IgnoreQueryFilters()
            //    .ToDictionaryAsync(c => c.Name, c => c);

            var categories = await context.Categories
    .IgnoreQueryFilters()
    .ToListAsync();

            var categoryMap = categories
                .GroupBy(c => c.Name)
                .ToDictionary(g => g.Key, g => g.First());

            var cities = await context.Cities
                .IgnoreQueryFilters()
                .ToListAsync();

            var cityMap = cities
                .GroupBy(c => c.Name)
                .ToDictionary(g => g.Key, g => g.First());
            var eventSeeds = new List<EventSeedItem>
            {
                new("Лятна музикална вечер", "Вечер с популярна българска музика и специални гост-изпълнители.", "София", "НДК, Зала 1", DateTime.Today.AddDays(10).AddHours(20), 1200, "Концерт", 18.90m, "/images/events/Лятна музикална вечер.png"),
                new("Комедия на сцена", "Съвременна комедийна постановка с динамичен сюжет и силен актьорски състав.", "София", "Народен театър", DateTime.Today.AddDays(7).AddHours(19), 350, "Театър", 14.50m, "/images/events/theater-sofia.jpg"),
                new("Кино премиера: Град под светлините", "Премиерна прожекция на нов драматичен филм.", "София", "Cinema City Paradise", DateTime.Today.AddDays(5).AddHours(20), 160, "Кино", 9.90m, "/images/events/cinema-sofia.jpg"),
                new("Дерби на столицата", "Футболен мач с голям интерес и очаквана силна атмосфера.", "София", "Национален стадион Васил Левски", DateTime.Today.AddDays(18).AddHours(18), 2500, "Спорт", 16.00m, "/images/events/sport-sofia.jpg"),
                new("Експозиция: Цветове и форми", "Съвременна художествена изложба с живопис и графика.", "София", "Градска художествена галерия", DateTime.Today.AddDays(4).AddHours(18), 200, "Изложба", 11.50m, "/images/events/exhibition-sofia.jpg"),

                new("Джаз вечер под тепетата", "Изискана джаз програма с модерни и класически изпълнения.", "Пловдив", "Дом на културата Борис Христов", DateTime.Today.AddDays(9).AddHours(20), 420, "Концерт", 17.50m, "/images/events/jazz-plovdiv.jpg"),
                new("Сценична комедия „Късен влак“", "Забавна история с неочаквани обрати и силно сценично присъствие.", "Пловдив", "Драматичен театър Пловдив", DateTime.Today.AddDays(12).AddHours(19), 260, "Комедия", 13.90m, "/images/events/comedy-plovdiv.jpg"),
                new("Филмова вечер: Незабравимо лято", "Романтична филмова история, подходяща за вечер с приятели.", "Пловдив", "Cinema City Plovdiv", DateTime.Today.AddDays(6).AddHours(20), 140, "Кино", 8.90m, "/images/events/cinema-plovdiv.jpg"),
                new("Балкански баскетболен турнир", "Турнир с участници от няколко балкански държави.", "Пловдив", "Колодрум Пловдив", DateTime.Today.AddDays(16).AddHours(17), 900, "Спорт", 15.00m, "/images/events/basketball-plovdiv.jpg"),
                new("Детски празник: Приказен ден", "Събитие за деца с анимация, игри и сценична програма.", "Пловдив", "Дом на културата Борис Христов", DateTime.Today.AddDays(21).AddHours(11), 280, "Детски събития", 9.50m, "/images/events/kids-plovdiv.jpg"),

                new("Морски ритми на живо", "Концертна програма с поп, рок и акустични изпълнения.", "Варна", "Летен театър Варна", DateTime.Today.AddDays(11).AddHours(20), 1100, "Концерт", 18.00m, "/images/events/concert-varna.jpg"),
                new("Смях до полунощ", "Комедийна постановка с много хумор и лека лятна атмосфера.", "Варна", "Фестивален и конгресен център", DateTime.Today.AddDays(14).AddHours(19), 300, "Комедия", 14.90m, "/images/events/theater-varna.jpg"),
                new("Кино премиера: Последният залез", "Приключенски филм с драматични елементи и силна визия.", "Варна", "Arena Mall Varna Cinema", DateTime.Today.AddDays(8).AddHours(21), 170, "Кино", 10.50m, "/images/events/cinema-varna.jpg"),
                new("Стендъп вечер на живо", "Вечер на стендъп комедия с популярни български комедианти.", "Варна", "Фестивален и конгресен център", DateTime.Today.AddDays(19).AddHours(20), 300, "Стендъп", 13.50m, "/images/events/standup-varna.jpg"),

                new("Рок вечер край морето", "Енергичен концерт с български рок групи и специални гости.", "Бургас", "Летен театър Бургас", DateTime.Today.AddDays(13).AddHours(20), 950, "Концерт", 17.90m, "/images/events/concert-burgas.jpg"),
                new("Филмов фестивал: Късо кино", "Селекция от късометражни филми на млади автори.", "Бургас", "Културен дом НХК", DateTime.Today.AddDays(20).AddHours(18), 180, "Фестивал", 8.50m, "/images/events/cinema-burgas.jpg"),

                new("Вечер на класическата музика", "Програма с камерна музика и емблематични произведения.", "Русе", "Доходно здание", DateTime.Today.AddDays(15).AddHours(19), 320, "Концерт", 15.50m, "/images/events/concert-ruse.jpg"),

                new("Балетна гала вечер", "Подбрана балетна програма с класически и съвременни изпълнения.", "Стара Загора", "Държавна опера Стара Загора", DateTime.Today.AddDays(17).AddHours(19), 380, "Опера и балет", 16.90m, "/images/events/opera-stara-zagora.jpg"),

                new("Акустична музикална вечер", "Интимен концерт с акустични аранжименти и авторска музика.", "Плевен", "Драматично-куклен театър Иван Радоев", DateTime.Today.AddDays(9).AddHours(20), 240, "Концерт", 12.90m, "/images/events/concert-pleven.jpg"),

                new("Фестивал на светлините", "Вечерно събитие с музика, светлини и историческа атмосфера.", "Велико Търново", "Крепост Царевец", DateTime.Today.AddDays(22).AddHours(20), 1800, "Фестивал", 19.90m, "/images/events/festival-tarnovo.jpg")
            };

            foreach (var seed in eventSeeds)
            {
                if (!categoryMap.TryGetValue(seed.CategoryName, out var category))
                {
                    Console.WriteLine($"[Events seed] Missing category: {seed.CategoryName}");
                    continue;
                }

                if (!cityMap.TryGetValue(seed.CityName, out var city))
                {
                    Console.WriteLine($"[Events seed] Missing city: {seed.CityName}");
                    continue;
                }

                var existingEvent = await context.Events
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(e =>
                        e.Title == seed.Title &&
                        e.City == seed.CityName &&
                        e.Venue == seed.Venue);

                if (existingEvent == null)
                {
                    context.Events.Add(new Event
                    {
                        Title = seed.Title,
                        Description = seed.Description,
                        City = city.Name,
                        CityId = city.Id,
                        Venue = seed.Venue,
                        Date = seed.Date,
                        Capacity = seed.Capacity,
                        ImageUrl = seed.ImageUrl,
                        CategoryId = category.Id,
                        Price = seed.Price,
                        IsDeleted = false,
                        DeletedAt = null
                    });
                }
                else
                {
                    existingEvent.Description = seed.Description;
                    existingEvent.City = city.Name;
                    existingEvent.CityId = city.Id;
                    existingEvent.Venue = seed.Venue;
                    existingEvent.Date = seed.Date;
                    existingEvent.Capacity = seed.Capacity;
                    existingEvent.ImageUrl = seed.ImageUrl;
                    existingEvent.CategoryId = category.Id;
                    existingEvent.Price = seed.Price;
                    existingEvent.IsDeleted = false;
                    existingEvent.DeletedAt = null;
                }
            }

            await context.SaveChangesAsync();
        }

        private sealed class EventSeedItem
        {
            public EventSeedItem(
                string title,
                string description,
                string cityName,
                string venue,
                DateTime date,
                int capacity,
                string categoryName,
                decimal price,
                string imageUrl)
            {
                Title = title;
                Description = description;
                CityName = cityName;
                Venue = venue;
                Date = date;
                Capacity = capacity;
                CategoryName = categoryName;
                Price = price;
                ImageUrl = imageUrl;
            }

            public string Title { get; }
            public string Description { get; }
            public string CityName { get; }
            public string Venue { get; }
            public DateTime Date { get; }
            public int Capacity { get; }
            public string CategoryName { get; }
            public decimal Price { get; }
            public string ImageUrl { get; }
        }
    }
}
