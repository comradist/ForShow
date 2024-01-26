using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObject;

public record EmployeeDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }

    public string Name { get; init; }

    public int Age { get; init; }

    public string Position { get; init; }
}