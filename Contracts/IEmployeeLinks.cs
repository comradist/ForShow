using Entities.LinkModels;
using Shared.DataTransferObject;
using Microsoft.AspNetCore.Http;

namespace Contracts;

public interface IEmployeeLinks
{
    LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeeDto, string fields, Guid companyId, HttpContext httpContext);
}