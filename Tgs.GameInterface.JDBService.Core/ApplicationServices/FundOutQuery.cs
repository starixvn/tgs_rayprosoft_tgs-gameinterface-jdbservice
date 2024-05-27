namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record FundOutQuery(
	Uri ApiUrl,
	string Domain,
	string AesKey,
	string AesIV,
	string Agent,
	string PlayerCode,
	string SerialNo,
	decimal Amount);
