namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record PlayerCreateCommand(
	Uri ApiUrl,
	string Domain,
	string AesKey,
	string AesIV,
	string Agent,
	string PlayerCode);
