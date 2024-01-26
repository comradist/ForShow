using System.ComponentModel.DataAnnotations;


namespace Shared.DataTransferObject;


public abstract record CompanyForManipulationDto
{
    [Required(ErrorMessage = "Company name is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Name is 60 characters.")]
    public string? Name { get; init; }

    [Required(ErrorMessage = "Address of company is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters.")]
    public string? Address { get; init; }

    [Required(ErrorMessage = "Country of company is a required field.")]
    [MaxLength(60, ErrorMessage = "Maximum length for the Address is 60 characters.")]
    public string? Country { get; init; }

    public IEnumerable<EmployeeForCreationDto>? Employees { get; init; }
}