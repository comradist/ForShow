using System.Text.Json;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Entities.LinkModels;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.DataTransferObject;
using Shared.RequestFeatures;

namespace CompanyEmployees.Presentation;


[Route("api/companies/{companyId}/employees")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IServiceManager serviceManager;
    public EmployeeController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpGet]
    [HttpHead]
    [ServiceFilter(typeof(ValidateMediaTypeAttribute))]
    public async Task<IActionResult> GetEmloyeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
    {
        var linkParams = new LinkParameters(employeeParameters, HttpContext);

        var result = await serviceManager.EmployeeService.GetEmployeesAsync(companyId, linkParams, false);

        Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(result.metaData));

        return result.linkResponse.HasLinks ? Ok(result.linkResponse.LinkedEntities) : Ok(result.linkResponse.ShapedEntities);
    }

    [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
    public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
    {
        var employee = await serviceManager.EmployeeService.GetEmployeeAsync(companyId, id, false);
        return Ok(employee);
    }

    [HttpGet("collection/({ids})", Name = "GetEmployeeCollection")]
    public async Task<IActionResult> GetCreatedEmployeeCollectionForCompany(Guid companyId, [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
    {
        var emloyees = await serviceManager.EmployeeService.GetByIdsAsync(companyId, ids, false);
        return Ok(emloyees);
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
    {
        var emloyeeDto = await serviceManager.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, false);

        return CreatedAtRoute("GetEmployeeForCompany", new { companyId, employeeId = emloyeeDto.Id }, emloyeeDto);
    }

    [HttpPost("collection")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateEmployeesForCompany(Guid companyId, [FromBody] IEnumerable<EmployeeForCreationDto> employees)
    {
        var result = await serviceManager.EmployeeService.CreateEmployeesForCompanyAsync(companyId, employees, false);

        return CreatedAtRoute("GetEmployeeCollection", new {companyId, result.ids }, result.employeeDtos);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
    {
        await serviceManager.EmployeeService.DeleteEmploeeForCompanyAsync(companyId, id, false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
    {
        await serviceManager.EmployeeService.UpdateEmployeeForCompany(companyId, id, employee, false, true);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> pacthDoc)
    {
        if (pacthDoc is null)
        {
            return BadRequest("patchDoc object sent from client is null");
        }
        
        var result = await serviceManager.EmployeeService.GetEmployeeForPatchAsync(companyId, id, false, true);
        pacthDoc.ApplyTo(result.employeeToPacth, ModelState);

        TryValidateModel(result.employeeToPacth);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await serviceManager.EmployeeService.SaveChagesForPatch(result.employeeToPacth, result.employeeEntity);

        return NoContent();
    }
}