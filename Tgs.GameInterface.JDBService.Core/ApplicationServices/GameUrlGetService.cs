using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class GameUrlGetService : IGetService<GameUrlQuery, GameUrlResult>
{
    private readonly IGameApiClient _client;

    public GameUrlGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(
        GameUrlQuery query,
        CancellationToken cancellationToken = default)
    {
        (bool IsPlayerExist, string GameUrl) = await _client
                .GetTokenAsync(
                    query.ApiUrl,
                    query.Domain,
                    query.PartnerKey,
                    query.PartnerIV,
                    query.PlayerCode,
                    query.GameType,
                    query.MachineType,
                    query.Lang,
                    cancellationToken)
                .ConfigureAwait(false);
        return new GameUrlResult(
            IsPlayerExist: IsPlayerExist,
            GameUrl: GameUrl);
    }
}
