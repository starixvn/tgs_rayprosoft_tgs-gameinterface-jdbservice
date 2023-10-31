using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;
using Tgs.GameInterface.JDBService.Core.Models;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.ApplicationServices;

public class FundOutGetServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var query = new FundOutQuery(
            ApiUrl: new Uri("http://localhost/"),
            Domain: "ZF",
            AesKey: "831057181dd07807",
            AesIV: "193a3159afebee65",
            Agent: "zf055gpi",
            PlayerCode: "4ucmqplxhfoe",
            SerialNo: Guid.NewGuid().ToString("N"),
            Amount: 1);

        _ = fakeClient
            .DepositAsync(
            Arg.Is(query.ApiUrl),
            Arg.Is(query.Domain),
            Arg.Is(query.AesKey),
            Arg.Is(query.AesIV),
            Arg.Is(query.Agent),
            Arg.Is(query.PlayerCode),
            Arg.Is(query.SerialNo),
            Arg.Is(query.Amount),
            Arg.Any<CancellationToken>()
            ).Returns(new TransactionResp(
                SerialNo: query.SerialNo,
                Resp: new JsonObject
                {
                    ["serialNo"] = query.SerialNo
                }.ToJsonString()));

        var service = new FundOutGetService(
            fakeClient);

        var actual = await service.GetAsync(query: query);

        Assert.Equal(query.SerialNo, actual.SerialNo);
    }
}
