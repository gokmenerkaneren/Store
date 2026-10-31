using Dapper;
using Store.Discount.Context;
using Store.Discount.Dtos;
using System.Runtime.Versioning;

namespace Store.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly DapperContext _context;
        public DiscountService(DapperContext context)
        {
            _context = context;
        }

        public async Task CreateCouponAsync(CreateCouponDto createCouponDto)
        {
            var newId = Guid.NewGuid();
            var insertDate = Store.Shared.Entities.BaseEntity.GetLocalTime();
            string query = "INSERT INTO \"Coupons\" (\"Id\", \"Code\", \"Rate\", \"IsActive\", \"ValidDate\", \"InsertDate\", \"IsDeleted\") VALUES (@id, @code, @rate, @isActive, @validDate, @insertDate, false)";
            var parameters = new DynamicParameters();

            parameters.Add("@id", newId);
            parameters.Add("@code", createCouponDto.Code);
            parameters.Add("@rate", createCouponDto.Rate);
            parameters.Add("@isActive", createCouponDto.IsActive);
            parameters.Add("@validDate", createCouponDto.ValidDate);
            parameters.Add("@insertDate", insertDate);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task DeleteCouponAsync(Guid id)
        {
            var deleteDate = Store.Shared.Entities.BaseEntity.GetLocalTime();
            string query = "UPDATE \"Coupons\" SET \"IsDeleted\" = true, \"IsActive\" = false, \"DeleteDate\" = @deleteDate WHERE \"Id\" = @id";
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.Add("@deleteDate", deleteDate);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }

        public async Task<List<ResultCouponDto>> GetAllCouponsAsync()
        {
            string query = "SELECT * FROM \"Coupons\" WHERE \"IsDeleted\" = false";
            using (var connection = _context.CreateConnection())
            {
                var values = await connection.QueryAsync<ResultCouponDto>(query);
                return values.ToList();
            }
        }

        public async Task<GetByIdCouponDto> GetByIdCouponAsync(Guid id)
        {
            string query = "SELECT * FROM \"Coupons\" WHERE \"Id\" = @id AND \"IsDeleted\" = false";
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            using (var connection = _context.CreateConnection())
            {
                var values = await connection.QueryFirstOrDefaultAsync<GetByIdCouponDto>(query, parameters);
                return values;
            }
        }

        public async Task<ResultCouponDto?> GetByCodeCouponAsync(string code)
        {
            string query = "SELECT * FROM \"Coupons\" WHERE UPPER(\"Code\") = UPPER(@code) AND \"IsDeleted\" = false AND \"IsActive\" = true";
            var parameters = new DynamicParameters();
            parameters.Add("@code", code);
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<ResultCouponDto>(query, parameters);
            }
        }

        public async Task UpdateCouponAsync(UpdateCouponDto updateCouponDto)
        {
            var updateDate = Store.Shared.Entities.BaseEntity.GetLocalTime();
            string query = "UPDATE \"Coupons\" SET \"Code\" = @code, \"Rate\" = @rate, \"IsActive\" = @isActive, \"ValidDate\" = @validDate, \"UpdateDate\" = @updateDate WHERE \"Id\" = @id AND \"IsDeleted\" = false";
            var parameters = new DynamicParameters();
            parameters.Add("@code", updateCouponDto.Code);
            parameters.Add("@rate", updateCouponDto.Rate);
            parameters.Add("@isActive", updateCouponDto.IsActive);
            parameters.Add("@validDate", updateCouponDto.ValidDate);
            parameters.Add("@updateDate", updateDate);
            parameters.Add("@id", updateCouponDto.Id);
            using (var connection = _context.CreateConnection())
            {
                await connection.ExecuteAsync(query, parameters);
            }
        }
    }
}
