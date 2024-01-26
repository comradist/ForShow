using System.Dynamic;
using AutoMapper;
using Contracts;
using Entities.Exception;
using Entities.LinkModels;
using Entities.Models;
using Services.Contracts;
using Shared.DataTransferObject;
using Shared.RequestFeatures;

namespace Services;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager repository;

    private readonly ILoggerManager logger;

    private readonly IMapper mapper;

    private readonly IEmployeeLinks employeeLinks;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper, IEmployeeLinks employeeLinks)
    {
        this.repository = repository;
        this.logger = logger;
        this.mapper = mapper;
        this.employeeLinks = employeeLinks;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);

        var employeDto = mapper.Map<EmployeeDto>(employee);
        return employeDto;
    }

    public async Task<(LinkResponse linkResponse, MetaData metaData)> GetEmployeesAsync(Guid companyId, LinkParameters linkParameters, bool trackChanges)
    {
        if(!linkParameters.EmployeeParameters.ValidAgeRange)
        {
            throw new MaxAgeRangeBadRequestException();
        }

        await CheckIfCompanyExists(companyId, trackChanges);

        var employesWithMetaData = await repository.Employee.GetEmployeesAsync(companyId, linkParameters.EmployeeParameters, trackChanges);

        var employeesDto = mapper.Map<IEnumerable<EmployeeDto>>(employesWithMetaData);

        var linkResponse = employeeLinks.TryGenerateLinks(employeesDto, linkParameters.EmployeeParameters.Fields, companyId, linkParameters.Context);

        return (linkResponse, employesWithMetaData.MetaData);
    }

    public async Task<IEnumerable<EmployeeDto>> GetByIdsAsync(Guid company, IEnumerable<Guid> ids, bool trackChanges)
    {
        await CheckIfCompanyExists(company, trackChanges);

        var emloyees = await repository.Employee.GetByIdsAsync(company, ids, trackChanges);

        var emloyeesDto = mapper.Map<IEnumerable<EmployeeDto>>(emloyees);

        return emloyeesDto;
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeDto, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var emloyee = mapper.Map<Employee>(employeeDto);

        await repository.Employee.CreateEmployeeForCompanyAsync(companyId, emloyee);

        await repository.SaveChangesAsync();

        var employeToReturn = mapper.Map<EmployeeDto>(emloyee);

        return employeToReturn;
    }

    public async Task<(IEnumerable<EmployeeDto> employeeDtos, string ids)> CreateEmployeesForCompanyAsync(Guid companyId, IEnumerable<EmployeeForCreationDto> employeesDto, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employeesEntities = mapper.Map<IEnumerable<Employee>>(employeesDto);
        foreach(var emloyee in employeesEntities)
        {
            await repository.Employee.CreateEmployeeForCompanyAsync(companyId, emloyee);
        }
        await repository.SaveChangesAsync();

        var employeesToReturn = mapper.Map <IEnumerable<EmployeeDto>>(employeesEntities);
        string ids = string.Join(",", employeesToReturn.Select(c => c.Id));

        return (employeesToReturn, ids);
    }

    public async Task DeleteEmploeeForCompanyAsync(Guid companyId, Guid employeeId, bool trackChanges)
    {
        await CheckIfCompanyExists(companyId, trackChanges);

        var employee = await GetEmployeeForCompanyAndCheckIfItExists(companyId, employeeId, trackChanges);

        repository.Employee.DeleteEmploeeForCompany(employee);

        await repository.SaveChangesAsync();
    }

    public async Task UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);

        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

        mapper.Map(employeeForUpdate, employeeEntity);

        await repository.SaveChangesAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPacth, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    {
        await CheckIfCompanyExists(companyId, compTrackChanges);

        var employeeEntity = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

        var employeeToPacth = mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        return (employeeToPacth, employeeEntity);
    }

    public async Task SaveChagesForPatch(EmployeeForUpdateDto employeeToPacth, Employee employeeEntity)
    {
        mapper.Map(employeeToPacth, employeeEntity);
        await repository.SaveChangesAsync();
    }

    private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
    {
        var company = await repository.Company.GetCompanyAsync(companyId, false);
        if (company is null)
        {
            throw new CompanyNotFoundException(companyId);
        }
    }

    private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid employeeId, bool trackChanges)
    {
        var employee = await repository.Employee.GetEmployeeAsync(companyId, employeeId, trackChanges);
        if (employee is null)
        {
            throw new EmployeeNotFoundException(employeeId);
        }
        return employee;
    }
}