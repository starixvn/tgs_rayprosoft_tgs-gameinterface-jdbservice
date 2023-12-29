using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerCreateCommandService(
    IGameApiClient client) : ICommandService<PlayerCreateCommand>
{
    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
        => client.CreatePlayerAsync(
            apiUrl: command.ApiUrl,
            domain: command.Domain,
            aesKey: command.AesKey,
            aesIV: command.AesIV,
            agent: command.Agent,
            playerCode: command.PlayerCode,
            cancellationToken: cancellationToken);
}
