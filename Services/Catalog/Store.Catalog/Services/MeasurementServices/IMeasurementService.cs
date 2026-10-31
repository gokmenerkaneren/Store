using Store.Catalog.Dtos.MeasurementDtos;

namespace Store.Catalog.Services.MeasurementServices
{
    public interface IMeasurementService
    {
        Task<List<ResultMeasurementDto>> GetAllMeasurementAsync(bool showDeleted = false);
        Task CreateMeasurementAsync(CreateMeasurementDto createMeasurementDto);
        Task UpdateMeasurementAsync(UpdateMeasurementDto updateMeasurementDto);
        Task DeleteMeasurementAsync(Guid id);
        Task RestoreMeasurementAsync(Guid id);
        Task<GetByIdMeasurementDto?> GetByIdMeasurementAsync(Guid id);
    }
}

