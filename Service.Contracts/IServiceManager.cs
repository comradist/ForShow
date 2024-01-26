namespace Services.Contracts;

public interface IServiceManager
{
    ICompanyService CompanyService { get; }
    
    IEmployeeService EmployeeService { get; }

    IAuthenticalService AuthenticalService { get; }
}