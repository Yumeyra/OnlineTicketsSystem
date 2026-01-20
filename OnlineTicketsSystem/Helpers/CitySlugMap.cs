namespace OnlineTicketsSystem.Helpers
{
    public class CitySlugMap
    {
        // slug (латиница) -> име (кирилица)
        private static readonly Dictionary<string, string> SlugToBg =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Sofia"] = "София",
                ["Plovdiv"] = "Пловдив",
                ["Varna"] = "Варна",
                ["Burgas"] = "Бургас",
                ["Razgrad"] = "Разград",
                ["Ruse"] = "Русе",
                ["StaraZagora"] = "Стара Загора",
                ["Pleven"] = "Плевен",
                ["Shumen"] = "Шумен",
                ["VelikoTarnovo"] = "Велико Търново",
                ["Dobrich"] = "Добрич",
                ["Sliven"] = "Сливен",
                ["Yambol"] = "Ямбол",
                ["Haskovo"] = "Хасково",
                ["Kardzhali"] = "Кърджали",
                ["Pernik"] = "Перник",
                ["Vratza"] = "Враца",
                ["Gabrovo"] = "Габрово",
                ["Targovishte"] = "Търговище",
                ["Montana"] = "Монтана",
                ["Kyustendil"] = "Кюстендил",
                ["Lovech"] = "Ловеч",
                ["Pazardzhik"] = "Пазарджик",
                ["Smolyan"] = "Смолян",
                ["Silistra"] = "Силистра",
                ["Vidin"] = "Видин",
                ["Blagoevgrad"] = "Благоевград"
            };

        // име (кирилица) -> slug (латиница)
        private static readonly Dictionary<string, string> BgToSlug =
            new(StringComparer.OrdinalIgnoreCase);

        static CitySlugMap()
        {
            foreach (var kv in SlugToBg)
                BgToSlug[kv.Value] = kv.Key;
        }

        public static bool TryGetBgName(string slug, out string bgName)
            => SlugToBg.TryGetValue(slug, out bgName);

        public static bool TryGetSlug(string bgName, out string slug)
            => BgToSlug.TryGetValue(bgName, out slug);
    }
}

