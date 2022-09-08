namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record GameOldHistoryQuery(
    Uri ApiUrl,
    string Domain,
    string AesKey,
    string AesIV,
    string Agent,
    DateTime StartTime,
    DateTime EndTime);
