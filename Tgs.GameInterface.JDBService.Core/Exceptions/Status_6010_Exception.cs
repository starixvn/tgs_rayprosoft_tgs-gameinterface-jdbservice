namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class Status_6010_Exception : Exception
{
    public Status_6010_Exception(decimal amount) : base($"Balance exceeds the upper limit.({amount})")
    {
    }
}
