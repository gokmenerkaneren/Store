using Store.Catalog.Dtos.CurrencyDtos;

namespace Store.Catalog.Services.CurrencyServices
{
    public interface ICurrencyService
    {
        Task<List<ResultCurrencyDto>> GetAllCurrencyAsync(bool showDeleted = false);
        Task CreateCurrencyAsync(CreateCurrencyDto createCurrencyDto);
        Task UpdateCurrencyAsync(UpdateCurrencyDto updateCurrencyDto);
        Task DeleteCurrencyAsync(Guid id);
        Task RestoreCurrencyAsync(Guid id);
        Task<GetByIdCurrencyDto?> GetByIdCurrencyAsync(Guid id);
    }
}

