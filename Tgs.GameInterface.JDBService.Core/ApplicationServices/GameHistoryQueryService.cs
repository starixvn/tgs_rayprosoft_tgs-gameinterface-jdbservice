using Tgs.GameInterface.JDBService.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class GameHistoryQueryService : IQueryService<GameHistoryQuery, GameHistoryResult>
{
    private readonly IGameApiClient _client;

    public GameHistoryQueryService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public IAsyncEnumerable<GameHistoryResult> QueryAsync(
        GameHistoryQuery query,
        CancellationToken cancellationToken = default)
        => _client
        .SearchGameTransactionDetailsAsync(
            query.ApiUrl,
            query.Domain,
            query.AesKey,
            query.AesIV,
            query.Agent,
            query.StartTime,
            query.EndTime,
            cancellationToken);
}
