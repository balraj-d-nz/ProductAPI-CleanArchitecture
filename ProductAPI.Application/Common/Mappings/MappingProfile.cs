using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using ProductAPI.Application.DTOs;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductCreateDto, Product>();
            CreateMap<Product, ProductResponseDto>();
            CreateMap<ProductUpdateDto, Product>().ReverseMap();
            CreateMap<ProductPatchDto, Product>()
               .ForMember(dest => dest.Name, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Name)))
               .ForMember(dest => dest.Description, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Description)))
               .ForMember(dest => dest.Price, opt => opt.Condition(src => src.Price.HasValue));
        }
    }
}
