using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
{
    public CompanyRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {    
    }

    public async Task<List<Company>> GetAllCompaniesAsync(bool trackChanges)
    {
        return await FindAll(trackChanges).OrderBy(c => c.Name).ToListAsync();
        
    }

    public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! How Contains been enumerated
        return await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();
    }

    public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanger)
    {
        return await FindByCondition(c => c.Id.Equals(companyId), trackChanger).SingleOrDefaultAsync();
    }

    public async Task CreateCompanyAsync(Company company)
    {
        await CreateAsync(company);
    }

    public void DeleteCompany(Company companyId)
    {
        Delete(companyId);
    }
}