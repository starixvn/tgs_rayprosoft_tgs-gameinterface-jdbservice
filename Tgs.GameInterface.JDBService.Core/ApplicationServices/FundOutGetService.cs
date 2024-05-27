using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class FundOutGetService(
	IGameApiClient client) : IGetService<FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(
		FundOutQuery query,
		CancellationToken cancellationToken = default)
	{
		var result = await client
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
