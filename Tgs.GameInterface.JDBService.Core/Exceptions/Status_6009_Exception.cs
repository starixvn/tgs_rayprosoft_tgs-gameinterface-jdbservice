namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class Status_6009_Exception : Exception
{
    public Status_6009_Exception(decimal amount) : base($"The deposit amount exceeds the upper limit.({amount})")
    {
    }
}
