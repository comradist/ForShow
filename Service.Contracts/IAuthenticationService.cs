using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObject;

namespace Services.Contracts;

public interface IAuthenticalService
{
    Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistrationDto);
}