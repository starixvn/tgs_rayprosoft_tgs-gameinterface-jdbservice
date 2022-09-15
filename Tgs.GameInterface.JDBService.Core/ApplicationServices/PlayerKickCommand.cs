namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record PlayerKickCommand(
    Uri ApiUrl,
    string Domain,
    string AesKey,
    string AesIV,
    string Agent,
    string PlayerCode);
