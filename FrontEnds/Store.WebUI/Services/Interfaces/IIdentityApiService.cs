using Refit;
using Store.DtoLayer.IdentityDtos.UserDtos;
using Store.DtoLayer.IdentityDtos.RoleDtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

namespace Store.WebUI.Services.Interfaces
{
    public interface IIdentityApiService
    {
        [Post("/api/users/check-exists")]
        Task<HttpResponseMessage> CheckUserExistsAsync([Body] Store.WebUI.Models.CheckUserExistsRequest request);

        [Get("/api/users")]
        Task<List<ResultUserDto>> GetAllUsersAsync();

        [Get("/api/users/{id}")]
        Task<ResultUserDto> GetUserByIdAsync(Guid id);

        [Post("/api/users")]
        Task<HttpResponseMessage> CreateUserAsync([Body] CreateUserDto createUserDto);

        [Put("/api/users")]
        Task<HttpResponseMessage> UpdateUserAsync([Body] UpdateUserDto updateUserDto);

        [Delete("/api/users/{id}")]
        Task<HttpResponseMessage> DeleteUserAsync(Guid id);

        [Get("/api/users/roles")]
        Task<List<ResultRoleDto>> GetAllRolesAsync();

        [Get("/api/users/{id}/roles")]
        Task<List<string>> GetUserRolesAsync(Guid id);

        [Post("/api/users/assign-role")]
        Task<HttpResponseMessage> AssignRoleAsync([Body] AssignRoleDto assignRoleDto);

        [Post("/connect/token")]
        [Headers("Content-Type: application/x-www-form-urlencoded")]
        Task<HttpResponseMessage> GetTokenAsync([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, string> data);
    }
}
