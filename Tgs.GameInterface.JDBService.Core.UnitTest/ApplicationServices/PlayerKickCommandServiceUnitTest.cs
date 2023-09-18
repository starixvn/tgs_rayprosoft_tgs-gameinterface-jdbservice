using NSubstitute;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.ApplicationServices;

public class PlayerKickCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeClient = Substitute.For<IGameApiClient>();

        var command = new PlayerKickCommand(
            ApiUrl: new Uri("http://localhost/"),
            Domain: "ZF",
            AesKey: "831057181dd07807",
            AesIV: "193a3159afebee65",
            Agent: "zf055gpi",
            PlayerCode: "4ucmqplxhfoe");

        var service = new PlayerKickCommandService(
            fakeClient);

        await service
            .ExecuteAsync(
            command: command)
            .ConfigureAwait(false);

        _ = fakeClient
            .Received(1)
            .KickOutPlayerAsync(
            Arg.Is(command.ApiUrl),
            Arg.Is(command.Domain),
            Arg.Is(command.AesKey),
            Arg.Is(command.AesIV),
            Arg.Is(command.Agent),
            Arg.Is(command.PlayerCode),
            Arg.Any<CancellationToken>());
    }
}
