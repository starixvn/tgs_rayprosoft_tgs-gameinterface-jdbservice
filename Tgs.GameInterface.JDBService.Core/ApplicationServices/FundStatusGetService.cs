using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class FundStatusGetService : IGetService<FundStatusQuery, bool>
{
    private readonly IGameApiClient _client;

    public FundStatusGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public ValueTask<bool> GetAsync
        (FundStatusQuery query,
        CancellationToken cancellationToken = default)
        => _client
        .CheckCashTransferAsync(
            query.ApiUrl,
            query.Domain,
            query.AesKey,
            query.AesIV,
            query.Agent,
            query.SerialNo,
            cancellationToken);
}
