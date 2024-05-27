using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerLogoutCommandService(IGameApiClient client) : ICommandService<PlayerLogoutCommand>
{
    public ValueTask ExecuteAsync(PlayerLogoutCommand command, CancellationToken cancellationToken = default)
        => client.PlayerLogoutAsync(
            apiUrl: command.ApiUrl,
            domain: command.Domain,
            aesKey: command.AesKey,
            aesIV: command.AesIV,
            agent: command.Agent,
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken);
}
