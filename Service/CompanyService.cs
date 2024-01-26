using AutoMapper;
using Contracts;
using Entities.Exception;
using Entities.Models;
using Services.Contracts;
using Shared.DataTransferObject;

namespace Services;

internal sealed class CompanyService : ICompanyService
{
    private readonly IRepositoryManager repository;

    private readonly ILoggerManager logger;

    private readonly IMapper mapper;

    public CompanyService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        this.repository = repository;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanger)
    {
        var companies = await repository.Company.GetAllCompaniesAsync(trackChanger);

        var companiesDto = mapper.Map<IEnumerable<CompanyDto>>(companies);
            
        return companiesDto;

    }

    public async Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
    {
        if (ids is null)
        {
            throw new IdParametersBadRequestException();
        }

        var companyEntities = await repository.Company.GetByIdsAsync(ids, trackChanges);
        if(ids.Count() != companyEntities.Count())
        {
            throw new CollectionByIdsBadRequestException();
        }

        var companiesToReturn = mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        return companiesToReturn;
    }

    public async Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfCompanyExists(companyId, trackChanges);

        var companyDto = mapper.Map<CompanyDto>(company);
        return companyDto;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto companyDto)
    {
        var company = mapper.Map<Company>(companyDto);

        await repository.Company.CreateCompanyAsync(company);
        await repository.SaveChangesAsync();

        var companyToReturn = mapper.Map<CompanyDto>(company);
        return companyToReturn;
    }

    public async Task<(IEnumerable<CompanyDto> companies, string ids)> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyColletion)
    {
        if (companyColletion is null)
        {
            throw new CompanyCollectionBadRequest();
        }
        var companyEntities = mapper.Map<IEnumerable<Company>>(companyColletion);
        foreach (var company in companyEntities)
        {
            await repository.Company.CreateCompanyAsync(company);
        }
        await repository.SaveChangesAsync();

        var companyColletionToReturn = mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
        var ids = string.Join(",", companyColletionToReturn.Select(c => c.Id));

        return (companies: companyColletionToReturn, ids: ids);
    }

    public async Task DeleteCompanyAsync(Guid companyId, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfCompanyExists(companyId, trackChanges);

        repository.Company.DeleteCompany(company);
        await repository.SaveChangesAsync();
    }

    public async Task UpdateCompany(Guid companyId, CompanyForUpdateDto companyForUpdate, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfCompanyExists(companyId, trackChanges);

        mapper.Map(companyForUpdate, company);
        await repository.SaveChangesAsync();
    }

    public async Task<(CompanyForUpdateDto companyToPatch, Company company)> GetCompayForPatch(Guid companyId, bool trackChanges)
    {
        var company = await GetCompanyAndCheckIfCompanyExists(companyId, trackChanges);

        var companiesDto = mapper.Map<CompanyForUpdateDto>(company);
        return (companiesDto, company);
    }

    public async Task SaveChagesForPatch(CompanyForUpdateDto companyToPatch, Company company)
    {
        mapper.Map(companyToPatch, company);
        await repository.SaveChangesAsync();
    }

    private async Task<Company> GetCompanyAndCheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, trackChanges);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
        return company;
    }
}