namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record PlayerBalanceQuery(
    Uri ApiUrl,
    string Domain,
    string PartnerKey,
    string PartnerIV,
    string Agent,
    string PlayerCode);
