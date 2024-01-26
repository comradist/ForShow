using Entities.Models;

namespace Contracts;

public interface ICompanyRepository
{
    Task<List<Company>> GetAllCompaniesAsync(bool trackChanges);

    Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges);

    Task CreateCompanyAsync(Company company);

    Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);

    void DeleteCompany(Company companyId);


}
