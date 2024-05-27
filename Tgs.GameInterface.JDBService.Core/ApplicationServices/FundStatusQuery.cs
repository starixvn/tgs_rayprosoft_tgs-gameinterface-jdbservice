namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record FundStatusQuery(
	Uri ApiUrl,
	string Domain,
	string AesKey,
	string AesIV,
	string Agent,
	string SerialNo);
