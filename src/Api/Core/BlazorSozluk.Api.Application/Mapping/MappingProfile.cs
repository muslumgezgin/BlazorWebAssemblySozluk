using System;
using AutoMapper;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Common.ViewModels.Queries;

namespace BlazorSozluk.Api.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginUserViewModel>().ReverseMap();
        }
    }
}

