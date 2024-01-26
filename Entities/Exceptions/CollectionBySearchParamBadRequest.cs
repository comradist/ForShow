
namespace Entities.Exception;

public sealed class CollectionBySearchParamBadRequest : BadRequestException
{
    public CollectionBySearchParamBadRequest(string? message) : base(message)
    {
    }
}