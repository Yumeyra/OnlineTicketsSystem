using Microsoft.EntityFrameworkCore;
using OnlineTicketsSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace OnlineTicketsSystem.Data.Seed
{
    public class EventSeeder
    {
        //public static async Task SeedAsync(ApplicationDbContext context)
        //{
            public static async Task SeedAsync(ApplicationDbContext context)
        {
            // ❗ Ако вече има събития – НЕ правим нищо
            if (await context.Events.IgnoreQueryFilters().AnyAsync())
                return;

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


             new("Лятна музикална вечер", "Вечер с популярна българска музика и специални гост-изпълнители.", "София", "НДК, Зала 1", new DateTime(2026, 5, 22, 20, 0, 0), 1200, "Концерт", 18.90m, "/uploads/events/Лятна музикална вечер.png"),
             new("Комедия на сцена", "Съвременна комедийна постановка с динамичен сюжет и силен актьорски състав.", "София", "Народен театър", new DateTime(2026, 5, 25, 19, 0, 0), 350, "Театър", 14.50m, "/uploads/events/theater-sofia.png"),
             new("Кино премиера: Град под светлините", "Премиерна прожекция на нов драматичен филм.", "София", "Cinema City Paradise", new DateTime(2026, 5, 28, 20, 0, 0), 160, "Кино", 9.90m, "/uploads/events/cinema-sofia.png"),
             new("Дерби на столицата", "Футболен мач с голям интерес и очаквана силна атмосфера.", "София", "Национален стадион Васил Левски", new DateTime(2026, 6, 2, 18, 0, 0), 2500, "Спорт", 16.00m, "/uploads/events/sport-sofia.jpg"),
             new("Експозиция: Цветове и форми", "Съвременна художествена изложба с живопис и графика.", "София", "Градска художествена галерия", new DateTime(2026, 6, 5, 18, 0, 0), 200, "Изложба", 11.50m, "/uploads/events/exhibition-sofia.jpg"),

          new("Джаз вечер под тепетата", "Изискана джаз програма с модерни и класически изпълнения.", "Пловдив", "Дом на културата Борис Христов", new DateTime(2026, 5, 23, 20, 0, 0), 420, "Концерт", 17.50m, "/uploads/events/jazz-plovdiv.jpg"),
          new("Сценична комедия „Късен влак“", "Забавна история с неочаквани обрати и силно сценично присъствие.", "Пловдив", "Драматичен театър Пловдив", new DateTime(2026, 5, 30, 19, 0, 0), 260, "Комедия", 13.90m, "/uploads/events/comedy-plovdiv.jpg"),
          new("Филмова вечер: Незабравимо лято", "Романтична филмова история, подходяща за вечер с приятели.", "Пловдив", "Cinema City Plovdiv", new DateTime(2026, 6, 1, 20, 0, 0), 140, "Кино", 8.90m, "/uploads/events/plowdiv-cinema.jpg"),
          new("Балкански баскетболен турнир", "Турнир с участници от няколко балкански държави.", "Пловдив", "Колодрум Пловдив", new DateTime(2026, 6, 10, 17, 0, 0), 900, "Спорт", 15.00m, "/uploads/events/basketball-plovdiv.jpg"),
         new("Детски празник: Приказен ден", "Събитие за деца с анимация, игри и сценична програма.", "Пловдив", "Дом на културата Борис Христов", new DateTime(2026, 6, 15, 11, 0, 0), 280, "Детски събития", 9.50m, "/uploads/events/kids-plovdiv.jpg"),

         new("Морски ритми на живо", "Концертна програма с поп, рок и акустични изпълнения.", "Варна", "Летен театър Варна", new DateTime(2026, 5, 24, 20, 0, 0), 1100, "Концерт", 18.00m, "/uploads/events/concert-varna.jpg"),
          new("Смях до полунощ", "Комедийна постановка с много хумор и лека лятна атмосфера.", "Варна", "Фестивален и конгресен център", new DateTime(2026, 5, 29, 19, 0, 0), 300, "Комедия", 14.90m, "/uploads/events/theater-varna.jpg"),
          new("Кино премиера: Последният залез", "Приключенски филм с драматични елементи и силна визия.", "Варна", "Arena Mall Varna Cinema", new DateTime(2026, 6, 6, 21, 0, 0), 170, "Кино", 10.50m, "/uploads/events/cinema-varna.jpg"),
         new("Стендъп вечер на живо", "Вечер на стендъп комедия с популярни български комедианти.", "Варна", "Фестивален и конгресен център", new DateTime(2026, 6, 12, 20, 0, 0), 300, "Стендъп", 13.50m, "/uploads/events/standup-varna.jpg"),

         new("Рок вечер край морето", "Енергичен концерт с български рок групи и специални гости.", "Бургас", "Летен театър Бургас", new DateTime(2026, 5, 27, 20, 0, 0), 950, "Концерт", 17.90m, "/uploads/events/concert-burgas.jpg"),
         new("Филмов фестивал: Късо кино", "Селекция от късометражни филми на млади автори.", "Бургас", "Културен дом НХК", new DateTime(2026, 6, 18, 18, 0, 0), 180, "Фестивал", 8.50m, "/uploads/events/cinema-burgas.jpg"),

new("Вечер на класическата музика", "Програма с камерна музика и емблематични произведения.", "Русе", "Доходно здание", new DateTime(2026, 6, 8, 19, 0, 0), 320, "Концерт", 15.50m, "/uploads/events/concert-ruse.jpg"),

new("Балетна гала вечер", "Подбрана балетна програма с класически и съвременни изпълнения.", "Стара Загора", "Държавна опера Стара Загора", new DateTime(2026, 6, 14, 19, 0, 0), 380, "Опера и балет", 16.90m, "/uploads/events/opera-stara-zagora.jpg"),

new("Акустична музикална вечер", "Интимен концерт с акустични аранжименти и авторска музика.", "Плевен", "Драматично-куклен театър Иван Радоев", new DateTime(2026, 5, 31, 20, 0, 0), 240, "Концерт", 12.90m, "/uploads/events/concert-pleven.jpg"),

new("Фестивал на светлините", "Вечерно събитие с музика, светлини и историческа атмосфера.", "Велико Търново", "Крепост Царевец", new DateTime(2026, 6, 20, 20, 0, 0), 1800, "Фестивал", 19.90m, "/uploads/events/festival-tarnovo.jpg"),
            new("Пролетен фолклорен фестивал",
    "Традиционен празник с танцови състави и автентична българска музика.",
    "Разград",
    "Амфитеатър Разград",
    new DateTime(2026, 5, 12, 18, 0, 0),
    600,
    "Фестивал",
    9.90m,
    "/uploads/events/folk-razgrad.jpg"),

new("Лятно кино под звездите",
    "Вечерна прожекция на български и европейски филми на открито.",
    "Шумен",
    "Парк Кьошковете",
    new DateTime(2026, 5, 15, 20, 30, 0),
    350,
    "Кино",
    7.50m,
    "/uploads/events/cinema-shumen.jpg"),

new("Рок концерт: Северен вятър",
    "Енергично рок шоу с местни групи и специални гости.",
    "Търговище",
    "Летен театър Търговище",
    new DateTime(2026, 5, 18, 20, 0, 0),
    900,
    "Концерт",
    14.90m,
    "/uploads/events/rock-targovishte.jpg"),

new("Комедийна вечер „Смях в петък“",
    "Стендъп комедия с участието на популярни български комедианти.",
    "Силистра",
    "Дом на културата",
    new DateTime(2026, 5, 24, 19, 30, 0),
    420,
    "Комедия",
    12.00m,
    "/uploads/events/comedy-silistra.jpg"),

new("Изложба: Светлина и сенки",
    "Съвременна арт изложба с фотографии и дигитални инсталации.",
    "Добрич",
    "Художествена галерия Добрич",
    new DateTime(2026, 5, 28, 17, 0, 0),
    200,
    "Изложба",
    6.50m,
    "/uploads/events/exhibition-dobrich.jpg"),

new("Детски театър: Приключения в гората",
    "Забавна постановка за деца с куклени персонажи и музика.Входът е безплатен за всички деца.",
    "Севлиево",
    "Дом на културата",
    new DateTime(2026, 5, 30, 11, 0, 0),
    180,
    "Детски събития",
    0.00m,
    "/uploads/events/kids-sevlievo.jpg"),

new("Футболен мач: Локомотив – Дунав",
    "Регионално дерби с много емоции и силна публика.",
    "Горна Оряховица",
    "Стадион Локомотив",
    new DateTime(2026, 6, 2, 18, 30, 0),
    2500,
    "Спорт",
    10.00m,
    "/uploads/events/football-gorna.jpg"),

new("Акустична вечер с китара",
    "Интимен концерт с авторска музика и акустични аранжименти.",
    "Казанлък",
    "Литературно-художествен музей",
    new DateTime(2026, 6, 4, 20, 0, 0),
    120,
    "Концерт",
    11.00m,
    "/uploads/events/acoustic-kazanlak.jpg"),

new("Театрална постановка „Сянката на времето“",
    "Драматична история с модерен прочит и силна актьорска игра.",
    "Хасково",
    "Драматично-куклен театър",
    new DateTime(2026, 6, 7, 19, 0, 0),
    300,
    "Театър",
    13.50m,
    "/uploads/events/theater-haskovo.jpg"),

new("Фестивал на уличната храна",
    "Гастрономическо събитие с участието на местни готвачи и food trucks.",
    "Ямбол",
    "Централен площад",
    new DateTime(2026, 6, 10, 12, 0, 0),
    2000,
    "Фестивал",
    4.00m,
    "/uploads/events/food-yambol.jpg")
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
                        StartTime = seed.Date.TimeOfDay,   
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
                    existingEvent.StartTime = seed.Date.TimeOfDay;
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
