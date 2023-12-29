using System.Runtime.CompilerServices;
using Tgs.GameInterface.JDBService.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

internal class GameWagerOldQueryService(
    IGameApiClient client) : IQueryService<GameWagerOldQuery, GameWagerResult>
{
    public async IAsyncEnumerable<GameWagerResult> QueryAsync(
        GameWagerOldQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var transactions = await client
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
            var freeGameState = transaction.HasFreegame == 1;
            var gambleState = transaction.HasGamble == 1;
            var bet = (gambleState) ? transaction.GambleBet : transaction.Bet;

            yield return new GameWagerResult(
                PlayerCode: transaction.PlayerId,
                ExternalWagerId: transaction.HistoryId.ToString(),
                ExternalGameId: $"{transaction.GameType}_{transaction.MachineType}",
                FreeGameState: freeGameState,
                GambleState: gambleState,
                ExternalCreatedOn: transaction.GameDate,
                ExternalClosedOn: transaction.LastModifyTime,
                Bet: bet,
                Settle: transaction.Win,
                WinLoss: transaction.Total,
                AdditionalData: transaction.AdditionalData);
        }
    }
}
