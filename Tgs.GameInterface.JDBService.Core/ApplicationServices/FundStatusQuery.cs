namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record FundStatusQuery(
    Uri ApiUrl,
    string Domain,
    string PartnerKey,
    string PartnerIV,
    string Agent,
    string SerialNo);
