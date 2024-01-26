
using CompanyEmployees.Presentation.ActionFilters;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.DataTransferObject;

namespace CompanyEmployees.Presentation;

[Route("api/authentication")]
[ApiController]
public class AuthenticalController : ControllerBase
{
    private readonly IServiceManager service;

    public AuthenticalController(IServiceManager service)
    {
        this.service = service;
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        var result = await service.AuthenticalService.RegisterUser(userForRegistration);

        if(!result.Succeeded)
        {
            foreach(var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        return StatusCode(201);
    }

}