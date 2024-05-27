using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerLogoutAllCommandService(IGameApiClient client) : ICommandService<PlayerLogoutAllCommand>
{
	public ValueTask ExecuteAsync(PlayerLogoutAllCommand command, CancellationToken cancellationToken = default)
		=> client.PlayerLogoutAllAsync(
			apiUrl: command.ApiUrl,
			domain: command.Domain,
			aesKey: command.AesKey,
			aesIV: command.AesIV,
			agent: command.Agent,
			cancellationToken: cancellationToken);
}
