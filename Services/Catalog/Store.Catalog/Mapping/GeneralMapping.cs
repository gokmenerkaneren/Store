using AutoMapper;
using Store.Catalog.Entities;
using Store.Catalog.Dtos.BrandDtos;
using Store.Catalog.Dtos.CategoryDtos;
using Store.Catalog.Dtos.ProductDtos;
using Store.Catalog.Dtos.ProductDetailDtos;
using Store.Catalog.Dtos.ProductImageDtos;
using Store.Catalog.Dtos.ProductPriceDtos;
using Store.Catalog.Dtos.PriceTypeDtos;
using Store.Catalog.Dtos.MeasurementDtos;
using Store.Catalog.Dtos.CurrencyDtos;
using Store.Catalog.Dtos.ColorDtos;
using Store.Catalog.Dtos.SizeDtos;
using Store.Catalog.Dtos.DefaultSlider1Dtos;
using Store.Catalog.Dtos.DefaultTrendDtos;
using Store.Catalog.Dtos.BestSellerDtos;
using Store.Catalog.Dtos.NewProductDtos;

namespace Store.Catalog.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Brand, ResultBrandDto>().ReverseMap();
            CreateMap<Brand, CreateBrandDto>().ReverseMap();
            CreateMap<Brand, UpdateBrandDto>().ReverseMap();
            CreateMap<Brand, GetByIdBrandDto>().ReverseMap();

            CreateMap<Category, ResultCategoryDto>().ReverseMap();
            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();
            CreateMap<Category, GetByIdCategoryDto>().ReverseMap();

            CreateMap<Measurement, ResultMeasurementDto>().ReverseMap();
            CreateMap<Measurement, CreateMeasurementDto>().ReverseMap();
            CreateMap<Measurement, UpdateMeasurementDto>().ReverseMap();
            CreateMap<Measurement, GetByIdMeasurementDto>().ReverseMap();

            CreateMap<Currency, ResultCurrencyDto>().ReverseMap();
            CreateMap<Currency, CreateCurrencyDto>().ReverseMap();
            CreateMap<Currency, UpdateCurrencyDto>().ReverseMap();
            CreateMap<Currency, GetByIdCurrencyDto>().ReverseMap();

            CreateMap<Product, ResultProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Product, GetByIdProductDto>().ReverseMap();

            CreateMap<ProductAttribute, ProductAttributeDto>().ReverseMap();
            CreateMap<ProductVariant, ProductVariantDto>().ReverseMap();

            CreateMap<ProductImage, ResultProductImageDto>().ReverseMap();
            CreateMap<ProductImage, CreateProductImageDto>().ReverseMap();
            CreateMap<ProductImage, UpdateProductImageDto>().ReverseMap();
            CreateMap<ProductImage, GetByIdProductImageDto>().ReverseMap();

            CreateMap<ProductPrice, ResultProductPriceDto>().ReverseMap();
            CreateMap<ProductPrice, CreateProductPriceDto>().ReverseMap();
            CreateMap<ProductPrice, UpdateProductPriceDto>().ReverseMap();
            CreateMap<ProductPrice, GetByIdProductPriceDto>().ReverseMap();

            CreateMap<PriceType, ResultPriceTypeDto>().ReverseMap();
            CreateMap<PriceType, CreatePriceTypeDto>().ReverseMap();
            CreateMap<PriceType, UpdatePriceTypeDto>().ReverseMap();
            CreateMap<PriceType, GetByIdPriceTypeDto>().ReverseMap();

            // Color mappings - Renk lookup tablosu
            CreateMap<Color, ResultColorDto>().ReverseMap();
            CreateMap<Color, CreateColorDto>().ReverseMap();
            CreateMap<Color, UpdateColorDto>().ReverseMap();
            CreateMap<Color, GetByIdColorDto>().ReverseMap();

            // Size mappings - Beden lookup tablosu
            CreateMap<Size, ResultSizeDto>().ReverseMap();
            CreateMap<Size, CreateSizeDto>().ReverseMap();
            CreateMap<Size, UpdateSizeDto>().ReverseMap();
            CreateMap<Size, GetByIdSizeDto>().ReverseMap();

            // DefaultSlider1 mappings
            CreateMap<DefaultSlider1, ResultDefaultSlider1Dto>().ReverseMap();
            CreateMap<DefaultSlider1, CreateDefaultSlider1Dto>().ReverseMap();
            CreateMap<DefaultSlider1, UpdateDefaultSlider1Dto>().ReverseMap();
            CreateMap<DefaultSlider1, GetByIdDefaultSlider1Dto>().ReverseMap();

            // DefaultTrend mappings
            CreateMap<DefaultTrend, ResultDefaultTrendDto>().ReverseMap();
            CreateMap<DefaultTrend, CreateDefaultTrendDto>().ReverseMap();
            CreateMap<DefaultTrend, UpdateDefaultTrendDto>().ReverseMap();
            CreateMap<DefaultTrend, GetByIdDefaultTrendDto>().ReverseMap();

            CreateMap<BestSeller, CreateBestSellerDto>().ReverseMap();
            CreateMap<BestSeller, UpdateBestSellerDto>().ReverseMap();
            CreateMap<BestSeller, GetByIdBestSellerDto>().ReverseMap();
            CreateMap<BestSeller, ResultBestSellerDto>().ReverseMap();

            CreateMap<NewProduct, CreateNewProductDto>().ReverseMap();
            CreateMap<NewProduct, UpdateNewProductDto>().ReverseMap();
            CreateMap<NewProduct, GetByIdNewProductDto>().ReverseMap();
            CreateMap<NewProduct, ResultNewProductDto>().ReverseMap();

            // InstagramPost mappings
            CreateMap<Store.Catalog.Entities.InstagramPost, Store.Catalog.Dtos.InstagramPostDtos.CreateInstagramPostDto>().ReverseMap();
            CreateMap<Store.Catalog.Entities.InstagramPost, Store.Catalog.Dtos.InstagramPostDtos.UpdateInstagramPostDto>().ReverseMap();
            CreateMap<Store.Catalog.Entities.InstagramPost, Store.Catalog.Dtos.InstagramPostDtos.ResultInstagramPostDto>().ReverseMap();
        }
    }
}
