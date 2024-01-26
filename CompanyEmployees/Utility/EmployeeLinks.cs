using System.Dynamic;
using Microsoft.Net.Http.Headers;
using Contracts;
using Entities.LinkModels;
using Shared.DataTransferObject;

namespace CompanyEmployees.Utility;

public class EmployeeLinks : IEmployeeLinks
{
    private readonly LinkGenerator _linkGenerator;
    private readonly IDataShaper<EmployeeDto> _dataShaper;

    public EmployeeLinks(LinkGenerator linkGenerator, IDataShaper<EmployeeDto> dataShaper)
    {
        _linkGenerator = linkGenerator;
        _dataShaper = dataShaper;
    }

    public LinkResponse TryGenerateLinks(IEnumerable<EmployeeDto> employeeDto, string fields, Guid companyId, HttpContext httpContext)
    {
        var shapedEmployees = ShapeData(employeeDto, fields);

        if(ShouldGenerateLinks(httpContext))
        {
            return ReturnLinkdedEmployees(employeeDto, fields, companyId, httpContext, shapedEmployees);
        }

        return ReturnShapedEmployees(shapedEmployees);
    }

    private List<ExpandoObject> ShapeData(IEnumerable<EmployeeDto> employeeDto, string fields) =>
        _dataShaper.ShapeData(employeeDto, fields).ToList();

    private bool ShouldGenerateLinks(HttpContext httpContext)
    {
        //httpContext.Request.Headers.Accept.Contains("")
        var mediaType = (MediaTypeHeaderValue)httpContext.Items["AcceptHeaderMediaType"]!;

        return mediaType.SubTypeWithoutSuffix.EndsWith("hateoas", StringComparison.InvariantCultureIgnoreCase);
    }

    private LinkResponse ReturnShapedEmployees(List<ExpandoObject> shapedEmployees) =>
        new LinkResponse { ShapedEntities = shapedEmployees };

    private LinkResponse ReturnLinkdedEmployees(IEnumerable<EmployeeDto> employeesDto, string fields, Guid companyId, HttpContext httpContext, List<ExpandoObject> shapedEmployees)
    {
        var employeeDtoList = employeesDto.ToList();

        for (int i = 0; i < employeeDtoList.Count(); i++)
        {
            var employeeLinks = CreateLinksForEmployee(httpContext, companyId, employeeDtoList[i].Id, fields);
            shapedEmployees[i].TryAdd("Links", employeeLinks);
        }

        var employeeCollection = new LinkCollectionWrapper<ExpandoObject>(shapedEmployees);
        var linkedEmployees = CreateLinksForEmployees(httpContext, employeeCollection);

        return new LinkResponse { HasLinks = true, LinkedEntities = linkedEmployees };
    }

    private List<Link> CreateLinksForEmployee(HttpContext httpContext, Guid companyId, Guid id, string fields = "")
    {
        var links = new List<Link>
        {
            new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeeForCompany", values: new { companyId, id, fields})!,
                 "self", "GET"),
            new Link(_linkGenerator.GetUriByAction(httpContext, "DeleteEmployeeForCompany", values: new { companyId, id })!,
                 "delete_employee", "DELETE"),
            new Link(_linkGenerator.GetUriByAction(httpContext, "UpdateEmployeeForCompany", values: new { companyId, id })!,
                 "update_employee", "PUT"),
            new Link(_linkGenerator.GetUriByAction(httpContext, "PartiallyUpdateEmployeeForCompany", values: new { companyId, id })!,
                 "partially_update_employee", "PATCH"),
        };

        return links;
    }

    private LinkCollectionWrapper<ExpandoObject> CreateLinksForEmployees(HttpContext httpContext, LinkCollectionWrapper<ExpandoObject> employeesWrapper)
    {
        employeesWrapper.Links.Add(new Link(_linkGenerator.GetUriByAction(httpContext, "GetEmployeesForCompany", values: new { })!,
            "self", "GET"));

        return employeesWrapper;
    }
}