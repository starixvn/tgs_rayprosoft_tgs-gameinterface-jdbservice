using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerKickCommandService(IGameApiClient client) : ICommandService<PlayerKickCommand>
{
    public ValueTask ExecuteAsync(PlayerKickCommand command, CancellationToken cancellationToken = default)
        => client.KickOutPlayerAsync(
            apiUrl: command.ApiUrl,
            domain: command.Domain,
            aesKey: command.AesKey,
            aesIV: command.AesIV,
            agent: command.Agent,
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken);
}
