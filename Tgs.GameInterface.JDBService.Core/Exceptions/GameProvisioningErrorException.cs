namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class GameProvisioningErrorException : Exception
{
    public GameProvisioningErrorException(string status, string errText) : base($"Game Provisioning Error Messages.(status:{status} | err_text:{errText})")
    {
    }
}
