using System;
using AutoMapper;
using ProductAPI.Models;
using ProductAPI.DTOs.Product;

namespace ProductAPI.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, CreateProductRequest>().ReverseMap();
            CreateMap<Product, UpdateProductRequest>().ReverseMap();
        }
    }
}

