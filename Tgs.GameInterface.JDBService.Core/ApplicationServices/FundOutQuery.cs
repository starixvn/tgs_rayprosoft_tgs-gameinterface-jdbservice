namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record FundOutQuery(
    Uri ApiUrl,
    string Domain,
    string PartnerKey,
    string PartnerIV,
    string Agent,
    string PlayerCode,
    string SerialNo,
    decimal Amount);
