using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using Shared.DataTransferObject;

namespace CompanyEmployees.Presentation;

//! GetCompaniesWithEmpoyeePage

[ApiVersion("1.0")]
[Route("api/companies")]
[ApiController]
public class CompaniesController : ControllerBase
{
    private readonly IServiceManager serviceManager;

    public CompaniesController(IServiceManager serviceManager)
    {
        this.serviceManager = serviceManager;
    }

    [HttpOptions]
    public IActionResult GetCompaniesOptions()
    {
        Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");
        return Ok();
    }

    [HttpGet(Name = "GetCompanies")]
    public async Task<IActionResult> GetCompanies()
    {
        var companies = await serviceManager.CompanyService.GetAllCompaniesAsync(trackChanger: false);
        return Ok(companies);
    }

    [HttpGet("{id:guid}", Name = "CompanyById")]
    //[ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetCompany(Guid id)
    {
        var company = await serviceManager.CompanyService.GetCompanyAsync(id, trackChanger: false);
        return Ok(company);
    }

    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
    {
        var companies = await serviceManager.CompanyService.GetByIdsAsync(ids, false);
        return Ok(companies);
    }

    [HttpPost(Name = "CreateCompany")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
    {
        var createdCompany = await serviceManager.CompanyService.CreateCompanyAsync(company);

        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    }

    [HttpPost("collection")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
    {
        var result = await serviceManager.CompanyService.CreateCompanyCollection(companyCollection);

        return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
    }


    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCompany(Guid id)
    {
        await serviceManager.CompanyService.DeleteCompanyAsync(id, false);
        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ServiceFilter(typeof(ValidationFilterAttribute))]
    public async Task<IActionResult> UpdateCompany(Guid id, [FromBody]CompanyForUpdateDto companyForUpdate)
    {
        await serviceManager.CompanyService.UpdateCompany(id, companyForUpdate, true);
        return NoContent();
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PartiallyUpdateCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDoc)
    {
        if(patchDoc is null)
        {
            return BadRequest("patchDoc object sent from client is null");
        }
        var result = await serviceManager.CompanyService.GetCompayForPatch(id, true);
        patchDoc.ApplyTo(result.companyToPatch, ModelState);

        TryValidateModel(result.companyToPatch);

        if (!ModelState.IsValid)
        {
            return UnprocessableEntity(ModelState);
        }

        await serviceManager.CompanyService.SaveChagesForPatch(result.companyToPatch, result.company);

        return NoContent();
    }
}