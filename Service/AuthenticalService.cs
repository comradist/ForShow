using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Services.Contracts;
using Shared.DataTransferObject;

namespace Services;

public sealed class AuthenticalService : IAuthenticalService
{
    private readonly IConfiguration configuration;

    private readonly UserManager<User> userManager;

    private readonly RoleManager<IdentityRole> roleManager;

    private readonly ILoggerManager logger;

    private readonly IMapper mapper;

    public AuthenticalService(ILoggerManager logger, IMapper mapper, RoleManager<IdentityRole> roleManager, UserManager<User> userManager, IConfiguration configuration)
    {
        this.logger = logger;
        this.mapper = mapper;
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.configuration = configuration;
    }

    public async Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration)
    {
        var user = mapper.Map<User>(userForRegistration);

        var userCreateResult = await userManager.CreateAsync(user, userForRegistration.Password);

        foreach(var role in userForRegistration.Roles)
        {
            var check = await roleManager.RoleExistsAsync(role);
            if(!check)
            {
                throw new InvalidDataException($"Role: {role}, is not exist.");
            }
        }

        if (userCreateResult.Succeeded)
        {
            await userManager.AddToRolesAsync(user, userForRegistration.Roles);
        }

        return userCreateResult;
    }
}