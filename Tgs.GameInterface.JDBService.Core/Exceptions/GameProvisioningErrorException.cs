namespace Tgs.GameInterface.JDBService.Core.Exceptions;

public class GameProvisioningErrorException(string status, string errText) : Exception($"Game Provisioning Error Messages.(status:{status} | err_text:{errText})")
{
}
