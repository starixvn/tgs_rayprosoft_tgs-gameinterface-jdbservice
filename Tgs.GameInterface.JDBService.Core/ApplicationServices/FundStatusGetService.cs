using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class FundStatusGetService(
    IGameApiClient client) : IGetService<FundStatusQuery, bool>
{
    public ValueTask<bool> GetAsync
        (FundStatusQuery query,
        CancellationToken cancellationToken = default)
        => client.CheckCashTransferAsync(
            apiUrl: query.ApiUrl,
            domain: query.Domain,
            aesKey: query.AesKey,
            aesIV: query.AesIV,
            agent: query.Agent,
            serialNo: query.SerialNo,
            cancellationToken: cancellationToken);
}
