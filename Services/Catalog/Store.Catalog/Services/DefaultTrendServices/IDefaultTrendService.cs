using Store.Catalog.Dtos.DefaultTrendDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.Catalog.Services.DefaultTrendServices
{
    public interface IDefaultTrendService
    {
        Task<List<ResultDefaultTrendDto>> GetAllDefaultTrendAsync();
        Task CreateDefaultTrendAsync(CreateDefaultTrendDto createDefaultTrendDto);
        Task UpdateDefaultTrendAsync(UpdateDefaultTrendDto updateDefaultTrendDto);
        Task DeleteDefaultTrendAsync(Guid id);
        Task<GetByIdDefaultTrendDto> GetByIdDefaultTrendAsync(Guid id);
    }
}