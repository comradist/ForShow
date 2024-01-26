namespace Shared.RequestFeatures;

public class EmployeeParameters : RequestParameters
{
    public EmployeeParameters() => OrderBy = "name";

    public bool ValidAgeRange => MaxAge > MinAge;

    public uint MinAge { get; set; }

    public uint MaxAge { get; set; } = int.MaxValue;

    public string? SearchTerm { get; set; }

}