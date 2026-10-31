using MediatR;
//using Store.Catalog.Dtos.ProductDtos;
using System.Collections.Generic;

namespace Store.DtoLayer.CatalogDtos.ProductDtos
{
    public class GetAllProductsQuery : IRequest<List<ResultProductDto>>
    {
        public bool ShowDeleted { get; set; } = false;
    }
}
