using MediatR;
using System.Collections.Generic;
using Store.Catalog.Dtos.ProductDtos;

namespace Store.Catalog.Queries;

public record GetAllProductsQuery(bool ShowDeleted = false) : IRequest<List<ResultProductDto>>;

