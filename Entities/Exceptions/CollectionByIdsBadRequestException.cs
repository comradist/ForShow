namespace Entities.Exception;

public sealed class CollectionByIdsBadRequestException : BadRequestException
{
    public CollectionByIdsBadRequestException() : base("Collection count mismatch comparing to ids")
    {
    }
}