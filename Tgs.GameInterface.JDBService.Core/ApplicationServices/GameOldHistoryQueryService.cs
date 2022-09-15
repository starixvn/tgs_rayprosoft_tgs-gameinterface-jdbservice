using Tgs.GameInterface.JDBService.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class GameOldHistoryQueryService : IQueryService<GameOldHistoryQuery, GameHistoryResult>
{
    private readonly IGameApiClient _client;

    public GameOldHistoryQueryService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public IAsyncEnumerable<GameHistoryResult> QueryAsync(
        GameOldHistoryQuery query,
        CancellationToken cancellationToken = default)
        => _client
        .SearchGameOldTransactionDetailsAsync(
            query.ApiUrl,
            query.Domain,
            query.AesKey,
            query.AesIV,
            query.Agent,
            query.StartTime,
            query.EndTime,
            cancellationToken);
}
