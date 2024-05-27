namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;
public record PlayerLogoutAllCommand(
	Uri ApiUrl,
	string Domain,
	string AesKey,
	string AesIV,
	string Agent);
