using AutoMapper;
using SORD.Library.API.Test.Dtos;
using SORD.Library.API.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SORD.Library.API.Test.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserReadDto>();
        }
    }
}
