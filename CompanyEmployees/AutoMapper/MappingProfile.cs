using AutoMapper;
using Entities.Models;
using Shared.DataTransferObject;

namespace AutoMapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Company, CompanyDto>()
            .ForMember(c => c.FullAddress, opt => opt.MapFrom(x => string.Join(" ", x.Address, x.Country)));

        CreateMap<Employee, EmployeeDto>().ReverseMap();

        CreateMap<CompanyForCreationDto, Company>();

        CreateMap<CompanyForUpdateDto, Company>().ReverseMap();

        CreateMap<EmployeeForCreationDto, Employee>();

        CreateMap<EmployeeForUpdateDto, Employee>().ReverseMap();

        CreateMap<UserForRegistrationDto, User>();

    }
}
