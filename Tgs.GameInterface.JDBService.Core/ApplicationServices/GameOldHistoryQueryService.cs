using System.Runtime.CompilerServices;
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

    public async IAsyncEnumerable<GameHistoryResult> QueryAsync(
        GameOldHistoryQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var transactions = await _client
            .SearchGameOldTransactionDetailsAsync(
            apiUrl: query.ApiUrl,
            domain: query.Domain,
            aesKey: query.AesKey,
            aesIV: query.AesIV,
            agent: query.Agent,
            startTime: query.StartTime,
            endTime: query.EndTime,
            cancellationToken: cancellationToken)
            .ToArrayAsync(
            cancellationToken: cancellationToken)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameDetailTransactionResp>();

        if (!transactions.Any())
        {
            yield break;
        }

        foreach (var transaction in transactions)
        {
            var bet = (transaction.HasGamble == 0) ? transaction.Bet : transaction.GambleBet;

            yield return new GameHistoryResult(
                PlayerCode: transaction.PlayerId,
                ExternalRoundId: transaction.HistoryId.ToString(),
                ExternalGameId: $"{transaction.GameType}_{transaction.MachineType}",
                ExternalCreatedOn: transaction.GameDate,
                ExternalClosedOn: transaction.LastModifyTime,
                Bet: bet,
                ValidBet: bet,
                PlayerWinLoss: transaction.Total,
                Turnover: bet,
                Settle: transaction.Win,
                AdditionalData: transaction.AdditionalData);
        }
    }
}
