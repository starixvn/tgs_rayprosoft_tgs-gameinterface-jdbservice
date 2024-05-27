using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class GameUrlGetService(
	IGameApiClient client) : IGetService<GameUrlQuery, GameUrlResult>
{
	public async ValueTask<GameUrlResult> GetAsync(
		GameUrlQuery query,
		CancellationToken cancellationToken = default)
	{
		(bool IsPlayerExist, string GameUrl) = await client
				.GetTokenAsync(
					query.ApiUrl,
					query.Domain,
					query.AesKey,
					query.AesIV,
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
