using Entities.Models;
using Shared.RequestFeatures;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);

        Task<Employee> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);

        Task<IEnumerable<Employee>> GetByIdsAsync(Guid company, IEnumerable<Guid> ids, bool trackChanges);

        Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee);

        void DeleteEmploeeForCompany(Employee employeeId);
    }
}