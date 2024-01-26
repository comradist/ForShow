using System.Dynamic;
using Entities.LinkModels;
using Entities.Models;
using Shared.DataTransferObject;
using Shared.RequestFeatures;

namespace Services.Contracts;

public interface IEmployeeService
{
    Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid company, LinkParameters employeeParameters, bool trackChanges);

    Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges);

    Task<IEnumerable<EmployeeDto>> GetByIdsAsync(Guid company, IEnumerable<Guid> ids, bool trackChanges);

    Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employee, bool trackChanges);

    Task<(IEnumerable<EmployeeDto> employeeDtos, string ids)> CreateEmployeesForCompanyAsync(Guid companyId, IEnumerable<EmployeeForCreationDto> employee, bool trackChanges);

    Task DeleteEmploeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges);

    Task UpdateEmployeeForCompany(Guid comapnyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges);

    Task<(EmployeeForUpdateDto employeeToPacth, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges);

    Task SaveChagesForPatch(EmployeeForUpdateDto employeeToPacth, Employee employeeEntity);
}