using MediatR;
using Store.Catalog.Dtos.ProductDtos;

namespace Store.Catalog.Commands;

public record CreateProductCommand(CreateProductDto CreateDto) : IRequest<ResultProductDto>;

