namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record GameUrlQuery(
    Uri ApiUrl,
    string Domain,
    string AesKey,
    string AesIV,
    string PlayerCode,
    string GameType,
    string MachineType,
    string Lang);
