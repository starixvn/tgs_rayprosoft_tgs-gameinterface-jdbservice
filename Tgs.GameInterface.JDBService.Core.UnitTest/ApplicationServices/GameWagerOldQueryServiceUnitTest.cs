using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;
using Tgs.GameInterface.JDBService.Core.Models;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.ApplicationServices;

public class GameWagerOldQueryServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameWagerOldQuery(
            ApiUrl: new Uri("http://localhost/"),
            Domain: "ZF",
            AesKey: "831057181dd07807",
            AesIV: "193a3159afebee65",
            Agent: "zf055gpi",
            StartTime: DateTime.Now.AddMinutes(-10),
            EndTime: DateTime.Now);

        var sysUtcNow = DateTimeOffset.UtcNow;
        var resp = new GameDetailTransactionResp[]
            {
                new GameDetailTransactionResp(
                HistoryId: sysUtcNow.ToUnixTimeMilliseconds().ToString(),
                PlayerId:"test",
                GameType:1,
                MachineType:0,
                GameDate:sysUtcNow,
                Bet:10,
                Win:20,
                Total:10,
                Currency:"PP",
                Jackpot:0.0m,
                JackpotContribute:0.0m,
                Denom:0.1m,
                LastModifyTime:sysUtcNow,
                PlayerIp:"::1",
                ClientType:"PC",
                HasFreegame:0,
                HasGamble:0,
                GambleBet:0,
                SystemTakeWin:0,
                AdditionalData:new JsonObject
                {
                    ["historyId"] = sysUtcNow.ToUnixTimeMilliseconds()
                }.ToJsonString())
            };
        _ = fakeClient
            .SearchGameOldTransactionDetailsAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Domain),
            Arg.Is(query.AesKey),
            Arg.Is(query.AesIV),
            Arg.Is(query.Agent),
            Arg.Is(query.StartTime),
            Arg.Is(query.EndTime),
            Arg.Any<CancellationToken>()
            ).Returns(resp.ToAsyncEnumerable());

        var service = new GameWagerOldQueryService(
            fakeClient);

        var actual = await service.QueryAsync(
            query: query)
            .ToArrayAsync()
            ?? Enumerable.Empty<GameWagerResult>();

        Assert.Equal(resp.ToArray().ElementAt(0).PlayerId, actual.ElementAt(0).PlayerCode);
        Assert.Equal(resp.ToArray().ElementAt(0).HistoryId.ToString(), actual.ElementAt(0).ExternalWagerId);
        Assert.Equal($"{resp.ToArray().ElementAt(0).GameType}_{resp.ToArray().ElementAt(0).MachineType}", actual.ElementAt(0).ExternalGameId);
        Assert.False(actual.ElementAt(0).FreeGameState);
        Assert.False(actual.ElementAt(0).GambleState);
        Assert.Equal(resp.ToArray().ElementAt(0).GameDate, actual.ElementAt(0).ExternalCreatedOn);
        Assert.Equal(resp.ToArray().ElementAt(0).LastModifyTime, actual.ElementAt(0).ExternalClosedOn);
        Assert.Equal(resp.ToArray().ElementAt(0).Bet, actual.ElementAt(0).Bet);
        Assert.Equal(resp.ToArray().ElementAt(0).Total, actual.ElementAt(0).WinLoss);
        Assert.Equal(resp.ToArray().ElementAt(0).Win, actual.ElementAt(0).Settle);
        Assert.Equal(resp.ToArray().ElementAt(0).AdditionalData, actual.ElementAt(0).AdditionalData);
    }
}
