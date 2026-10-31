using Refit;
using Store.DtoLayer.OrderDtos;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Store.WebUI.Services.Interfaces
{
    public interface IOrderApiService
    {
        [Post("/api/Addresses")]
        Task<HttpResponseMessage> CreateAddressAsync([Body] CreateOrderAddressDto command);

        [Post("/api/Orderings")]
        Task<HttpResponseMessage> CreateOrderingAsync([Body] CreateOrderingDto command);

        [Post("/api/OrderDetails")]
        Task<HttpResponseMessage> CreateOrderDetailAsync([Body] CreateOrderDetailDto command);
    }
}

