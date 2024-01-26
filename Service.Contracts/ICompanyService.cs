using Entities.Models;
using Shared.DataTransferObject;

namespace Services.Contracts;

public interface ICompanyService
{
    Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanger);

    Task<CompanyDto> GetCompanyAsync(Guid Id, bool trackChanger);

    Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);

    Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyColletion);

    Task DeleteCompanyAsync(Guid companyId, bool trackChanges);

    Task UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges);

    Task<(CompanyForUpdateDto companyToPatch, Company company)> GetCompayForPatch(Guid id, bool trackChanges);

    Task SaveChagesForPatch(CompanyForUpdateDto companyToPatch, Company company);
}