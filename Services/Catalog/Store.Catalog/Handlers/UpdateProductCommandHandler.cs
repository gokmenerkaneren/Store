using MediatR;
using Store.Catalog.Commands;
using Store.Catalog.Dtos.ProductDtos;
using Store.Catalog.Services.ProductServices;

namespace Store.Catalog.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        await _productService.UpdateProductAsync(request.UpdateDto);
        return Unit.Value;
    }
}

