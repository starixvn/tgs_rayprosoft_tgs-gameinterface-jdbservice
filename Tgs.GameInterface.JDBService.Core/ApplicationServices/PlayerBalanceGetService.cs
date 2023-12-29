using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerBalanceGetService(
    IGameApiClient client) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
    public async ValueTask<PlayerBalanceResult> GetAsync(
        PlayerBalanceQuery query,
        CancellationToken cancellationToken = default)
    {
        (bool IsPlayerExist, decimal Balance) = await client
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
