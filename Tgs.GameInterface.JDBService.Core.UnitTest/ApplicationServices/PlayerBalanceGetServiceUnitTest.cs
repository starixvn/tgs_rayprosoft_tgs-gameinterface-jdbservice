using NSubstitute;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.ApplicationServices;

public class PlayerBalanceGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new PlayerBalanceQuery(
            ApiUrl: new Uri("http://localhost/"),
            Domain: "ZF",
            AesKey: "831057181dd07807",
            AesIV: "193a3159afebee65",
            Agent: "zf055gpi",
            PlayerCode: "4ucmqplxhfoe");

        _ = fakeClient
            .SearchPlayerInformationAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Domain),
            Arg.Is(query.AesKey),
            Arg.Is(query.AesIV),
            Arg.Is(query.Agent),
            Arg.Is(query.PlayerCode),
            Arg.Any<CancellationToken>()
            ).Returns((IsPlayerExist: true, Balance: 1.0m));

        var service = new PlayerBalanceGetService(
            fakeClient);

        var actual = await service
            .GetAsync(
            query: query)
            .ConfigureAwait(false);

        Assert.True(actual.IsPlayerExist);
        Assert.Equal(1.0m, actual.Balance);
    }
}
