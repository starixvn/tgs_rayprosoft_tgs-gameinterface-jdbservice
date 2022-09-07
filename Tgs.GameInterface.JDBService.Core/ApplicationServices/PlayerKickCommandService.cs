using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerKickCommandService : ICommandService<PlayerKickCommand>
{
    private readonly IGameApiClient _client;

    public PlayerKickCommandService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask ExecuteAsync(PlayerKickCommand command, CancellationToken cancellationToken = default)
        => _client
            .KickOutPlayerAsync(
                command.ApiUrl,
                command.Domain,
                command.PartnerKey,
                command.PartnerIV,
                command.Agent,
                command.PlayerCode,
                cancellationToken);
}
