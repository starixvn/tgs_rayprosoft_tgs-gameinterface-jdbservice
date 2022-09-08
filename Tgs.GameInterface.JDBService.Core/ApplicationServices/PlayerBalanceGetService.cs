using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IGameApiClient _client;

    public PlayerBalanceGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(
        PlayerBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        (bool IsPlayerExist, decimal Balance) = await _client
                .SearchPlayerInformationAsync(
                    query.ApiUrl,
                    query.Domain,
                    query.AesKey,
                    query.AesIV,
                    query.Agent,
                    query.PlayerCode,
                    cancellationToken)
                .ConfigureAwait(false);
        return new PlayerBalanceResult(
            IsPlayerExist: IsPlayerExist,
            Balance: Balance);
    }
}
