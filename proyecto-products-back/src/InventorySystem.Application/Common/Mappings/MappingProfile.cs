using AutoMapper;
using InventorySystem.Application.DTOs;
using InventorySystem.Domain.Entities;

namespace InventorySystem.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductListDto>();
        CreateMap<CreateProductDto, Product>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.IsActive, o => o.Ignore());

        CreateMap<User, UserDto>();
        CreateMap<Notification, NotificationDto>();
        CreateMap<Category, CategoryDto>();
    }
}
