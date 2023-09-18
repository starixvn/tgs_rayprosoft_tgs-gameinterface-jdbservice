using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class FundOutGetService : IGetService<FundOutQuery, FundResult>
{
    private readonly IGameApiClient _client;

    public FundOutGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundResult> GetAsync(
        FundOutQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _client
                .DepositAsync(
                    query.ApiUrl,
                    query.Domain,
                    query.AesKey,
                    query.AesIV,
                    query.Agent,
                    query.PlayerCode,
                    query.SerialNo,
                    query.Amount,
                    cancellationToken)
                .ConfigureAwait(false);
        return new FundResult(
            SerialNo: result.SerialNo,
            Resp: result.Resp);
    }
}
