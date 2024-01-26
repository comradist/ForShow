namespace Entities.Exception;

public sealed class IdParametersBadRequestException : BadRequestException
{
    public IdParametersBadRequestException() : base("Parametr ids is null")
    {
    }
}