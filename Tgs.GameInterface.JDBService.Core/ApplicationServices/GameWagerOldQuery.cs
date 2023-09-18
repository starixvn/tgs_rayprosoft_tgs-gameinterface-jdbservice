namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record GameWagerOldQuery(
    Uri ApiUrl,
    string Domain,
    string AesKey,
    string AesIV,
    string Agent,
    DateTime StartTime,
    DateTime EndTime);
