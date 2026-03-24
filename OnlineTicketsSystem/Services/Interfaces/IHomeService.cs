using OnlineTicketsSystem.ViewModels;

namespace OnlineTicketsSystem.Services.Interfaces
{
    public interface  IHomeService
    {
        Task<HomeIndexVm> GetHomeDataAsync(
            int? categoryId,
            string? selectedRegion,
            string? selectedCity,
            string? dateRange,
            string? priceRange);
    }
}
