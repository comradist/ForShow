using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;

namespace Services;

public sealed class ServiceManager : IServiceManager
{
    private readonly Lazy<ICompanyService> companyService;

    private readonly Lazy<IEmployeeService> employeeService;

    private readonly Lazy<IAuthenticalService> authenticalService;

    public ServiceManager(IRepositoryManager repositoryManager, ILoggerManager logger, IMapper mapper, IEmployeeLinks employeeLinks, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration)
    {
        companyService = new Lazy<ICompanyService>(() => new CompanyService(repositoryManager, logger, mapper));
        employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, logger, mapper, employeeLinks));
        authenticalService = new Lazy<IAuthenticalService>(() => new AuthenticalService(logger, mapper, roleManager, userManager, configuration));
    }

    public ICompanyService CompanyService => companyService.Value;

    public IEmployeeService EmployeeService => employeeService.Value;

    public IAuthenticalService AuthenticalService => authenticalService.Value;
}