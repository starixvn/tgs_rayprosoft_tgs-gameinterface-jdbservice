namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class Status_6010_Exception(decimal amount) : Exception($"Balance exceeds the upper limit.({amount})")
{
}
