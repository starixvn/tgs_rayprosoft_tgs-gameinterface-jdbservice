namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record PlayerOnlineQuery(
	Uri ApiUrl,
	string Domain,
	string AesKey,
	string AesIV,
	string Agent,
	string PlayerCode);
