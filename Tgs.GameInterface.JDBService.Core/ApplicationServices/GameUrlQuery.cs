namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record GameUrlQuery(
    Uri ApiUrl,
    string Domain,
    string PartnerKey,
    string PartnerIV,
    string PlayerCode,
    string GameType,
    string MachineType,
    string Lang);
