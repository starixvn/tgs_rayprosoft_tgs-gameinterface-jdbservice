using NSubstitute;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.ApplicationServices;

public class GameUrlGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new GameUrlQuery(
            ApiUrl: new Uri("http://localhost/"),
            Domain: "ZF",
            AesKey: "831057181dd07807",
            AesIV: "193a3159afebee65",
            PlayerCode: "4ucmqplxhfoe",
            GameType: "7",
            MachineType: "7001",
            Lang: "cn");

        _ = fakeClient
            .GetTokenAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Domain),
            Arg.Is(query.AesKey),
            Arg.Is(query.AesIV),
            Arg.Is(query.PlayerCode),
            Arg.Is(query.GameType),
            Arg.Is(query.MachineType),
            Arg.Is(query.Lang),
            Arg.Any<CancellationToken>()
            ).Returns((IsPlayerExist: true, GameUrl: "http://localhost/"));

        var service = new GameUrlGetService(
            fakeClient);

        var actual = await service
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.True(actual.IsPlayerExist);
        Assert.Equal("http://localhost/", actual.GameUrl);
    }
}
