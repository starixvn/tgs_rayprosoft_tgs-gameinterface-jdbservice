namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record PlayerLogoutCommand(
	Uri ApiUrl,
	string Domain,
	string AesKey,
	string AesIV,
	string Agent,
	string PlayerCode);
