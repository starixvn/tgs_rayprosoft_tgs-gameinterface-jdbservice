using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class FundInGetService : IGetService<FundInQuery, FundResult>
{
    private readonly IGameApiClient _client;

    public FundInGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundResult> GetAsync(
        FundInQuery query,
        CancellationToken cancellationToken = default)
    {
        var resp = await _client
            .WithdrawalAsync(
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
            SerialNo: resp.SerialNo,
            Resp: resp.Resp);
    }
}
