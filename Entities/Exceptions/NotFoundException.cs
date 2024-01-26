namespace Entities.Exception;

public abstract class NotFoundException : System.Exception
{
    public NotFoundException(string ex) : base (ex)
    {

    }
}