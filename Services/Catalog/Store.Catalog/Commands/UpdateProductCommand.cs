using MediatR;
using Store.Catalog.Dtos.ProductDtos;

namespace Store.Catalog.Commands;

public record UpdateProductCommand(UpdateProductDto UpdateDto) : IRequest<Unit>;

