using System.Collections.Generic;

namespace OnlineTicketsSystem.ViewModels
{
    public class CitiesMenuVm
    {
        public List<string> Regions { get; set; } = new();
        public Dictionary<string, List<CityMenuItemVm>> CitiesByRegion { get; set; } = new();
    }

    public class CityMenuItemVm
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
    }
}