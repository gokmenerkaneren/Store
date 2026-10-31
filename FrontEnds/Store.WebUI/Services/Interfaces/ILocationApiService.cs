using Refit;
using Store.DtoLayer.LocationDtos.CityDtos;
using Store.DtoLayer.LocationDtos.CountryDtos;
using Store.DtoLayer.LocationDtos.DistrictDtos;
using Store.DtoLayer.LocationDtos.NeighborhoodDtos;
using Store.DtoLayer.LocationDtos.UserAddressDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface ILocationApiService
    {
        [Get("/api/UserAddress?showDeleted={showDeleted}")]
        Task<List<ResultUserAddressDto>> GetUserAddressesAsync(bool showDeleted = false);

        [Get("/api/UserAddress/{id}")]
        Task<ResultUserAddressDto> GetUserAddressByIdAsync(Guid id);

        [Post("/api/UserAddress")]
        Task<HttpResponseMessage> CreateUserAddressAsync([Body] CreateUserAddressDto createUserAddressDto);

        [Put("/api/UserAddress")]
        Task<HttpResponseMessage> UpdateUserAddressAsync([Body] UpdateUserAddressDto updateUserAddressDto);

        [Delete("/api/UserAddress/{id}")]
        Task<HttpResponseMessage> DeleteUserAddressAsync(Guid id);

        [Delete("/api/UserAddress/soft-delete/{id}")]
        Task<HttpResponseMessage> SoftDeleteUserAddressAsync(Guid id);

        [Post("/api/UserAddress/set-default/{id}?customerId={customerId}")]
        Task<HttpResponseMessage> SetDefaultUserAddressAsync(Guid id, Guid customerId);

        [Get("/api/Country")]
        Task<List<ResultCountryDto>> GetCountriesAsync();

        [Get("/api/City")]
        Task<List<ResultCityDto>> GetCitiesAsync();

        [Get("/api/District")]
        Task<List<ResultDistrictDto>> GetDistrictsAsync();

        [Get("/api/Neighborhood/ByDistrictId/{districtId}")]
        Task<List<ResultNeighborhoodDto>> GetNeighborhoodsByDistrictIdAsync(Guid districtId);

        [Post("/api/Iletisim")]
        Task<HttpResponseMessage> CreateMesajlarAsync([Body] Store.DtoLayer.LocationDtos.MesajlarDtos.CreateMesajlarDto createMesajlarDto);

        [Post("/api/BultenAboneligi")]
        Task<HttpResponseMessage> CreateBultenAboneligiAsync([Body] Store.DtoLayer.LocationDtos.BultenAboneligiDtos.CreateBultenAboneligiDto createBultenAboneligiDto);

        [Get("/api/BultenAboneligi/Check")]
        Task<bool> CheckBultenAboneligiAsync(string email);
    }
}
