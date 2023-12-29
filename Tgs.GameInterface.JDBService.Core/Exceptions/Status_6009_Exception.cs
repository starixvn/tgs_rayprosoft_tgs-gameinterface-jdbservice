namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class Status_6009_Exception(decimal amount) : Exception($"The deposit amount exceeds the upper limit.({amount})")
{
}
