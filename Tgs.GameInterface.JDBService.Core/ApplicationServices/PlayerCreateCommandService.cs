using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerCreateCommandService : ICommandService<PlayerCreateCommand>
{
    private readonly IGameApiClient _client;

    public PlayerCreateCommandService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
        => _client
            .CreatePlayerAsync(
                command.ApiUrl,
                command.Domain,
                command.AesKey,
                command.AesIV,
                command.Agent,
                command.PlayerCode,
                cancellationToken);
}
