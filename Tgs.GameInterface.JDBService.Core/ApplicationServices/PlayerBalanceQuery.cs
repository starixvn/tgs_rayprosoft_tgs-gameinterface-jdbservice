namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record PlayerBalanceQuery(
    Uri ApiUrl,
    string Domain,
    string AesKey,
    string AesIV,
    string Agent,
    string PlayerCode);
