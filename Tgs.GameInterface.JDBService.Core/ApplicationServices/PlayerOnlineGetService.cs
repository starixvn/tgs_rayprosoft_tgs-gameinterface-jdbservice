using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class PlayerOnlineGetService(
	IGameApiClient client) : IGetService<PlayerOnlineQuery, bool>
{
	public ValueTask<bool> GetAsync(PlayerOnlineQuery query, CancellationToken cancellationToken = default)
		=> client.PlayerOnlineAsync(
			apiUrl: query.ApiUrl,
			domain: query.Domain,
			aesKey: query.AesKey,
			aesIV: query.AesIV,
			agent: query.Agent,
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken);
}
