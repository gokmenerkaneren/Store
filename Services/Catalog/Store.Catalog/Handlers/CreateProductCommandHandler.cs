using MediatR;
using Store.Catalog.Commands;
using Store.Catalog.Dtos.ProductDtos;
using Store.Catalog.Services.ProductServices;

namespace Store.Catalog.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ResultProductDto>
{
    private readonly IProductService _productService;

    public CreateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<ResultProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Delegate to service which returns the created product DTO
        return await _productService.CreateProductAsync(request.CreateDto);
    }
}

