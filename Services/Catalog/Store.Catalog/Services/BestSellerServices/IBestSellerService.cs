using Store.Catalog.Dtos.BestSellerDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Catalog.Services.BestSellerServices
{
    public interface IBestSellerService
    {
        Task<List<ResultBestSellerDto>> GetAllBestSellerAsync();
        Task CreateBestSellerAsync(CreateBestSellerDto createBestSellerDto);
        Task UpdateBestSellerAsync(UpdateBestSellerDto updateBestSellerDto);
        Task DeleteBestSellerAsync(Guid id);
        Task<GetByIdBestSellerDto> GetByIdBestSellerAsync(Guid id);
    }
}
