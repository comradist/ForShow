using Contracts;
using Entities.Models;
using Repository.Extesions;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;

namespace Repository;

public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
{
    public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        return await FindByCondition(x => x.CompanyId.Equals(companyId) && x.Id.Equals(employeeId), trackChanges).SingleOrDefaultAsync();
    }

    public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
    {
        var emloyees = await FindByCondition(x => x.CompanyId.Equals(companyId), trackChanges)
        .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
        .Search(employeeParameters.SearchTerm!)
        .Sort(employeeParameters.OrderBy!)
        .ToListAsync();

        return PagedList<Employee>.ToPagedList(emloyees, employeeParameters.PageNumber, employeeParameters.PageSize);
    }

    public async Task<IEnumerable<Employee>> GetByIdsAsync(Guid companyId, IEnumerable<Guid> ids, bool trackChanges)
    {
        return await FindByCondition(x => x.CompanyId.Equals(companyId) && ids.Contains(x.Id), trackChanges).ToListAsync();
    }

    public async Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee)
    {
        employee.CompanyId = companyId;
        await CreateAsync(employee);
    }

    public void DeleteEmploeeForCompany(Employee employeeId)
    {
        Delete(employeeId);
    }
}