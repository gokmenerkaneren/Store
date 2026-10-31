using MediatR;
using Store.Catalog.Services.ProductServices;
using Store.Catalog.Dtos.ProductDtos;
using System.Collections.Generic;
using System.Threading;
using Store.Catalog.Queries;

namespace Store.Catalog.Handlers
{
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ResultProductDto>>
    {
        private readonly IProductService _productService;

        public GetAllProductsQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<List<ResultProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            return await _productService.GetAllProductAsync(request.ShowDeleted);
        }
    }
}

