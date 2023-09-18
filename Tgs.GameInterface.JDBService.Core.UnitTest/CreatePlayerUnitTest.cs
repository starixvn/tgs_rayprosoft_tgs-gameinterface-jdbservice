using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;

public class CreatePlayerUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ucmqplxhfoe";

        var gameInterfaceSetting = Options.Create(new GameInterfaceSettingOptions()
        {
            WithdrawalRetryCount = 1,
            WithdrawalIntervalSeconds = 1
        });
        var fakeLog = NullLogger<GameApiClient>.Instance;
        var fakeAesSystemClock = Substitute.For<ISystemClock>();
        var fakeAesCryptography = Substitute.For<IAesCryptography>();

        var currentTime = DateTimeOffset.UtcNow;

        _ = fakeAesSystemClock.UtcNow.Returns(currentTime);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 12,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            name = playerCode,
            credit_allocated = 0
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"0000\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        await client.CreatePlayerAsync(
               apiUrl,
               domain,
               aesKey,
               aesIV,
               agent,
               playerCode
            ).ConfigureAwait(false);
    }

    [Fact]
    public async Task Player_Already_Exists()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ucmqplxhfoe";

        var gameInterfaceSetting = Options.Create(new GameInterfaceSettingOptions()
        {
            WithdrawalRetryCount = 1,
            WithdrawalIntervalSeconds = 1
        });
        var fakeLog = NullLogger<GameApiClient>.Instance;
        var fakeAesSystemClock = Substitute.For<ISystemClock>();
        var fakeAesCryptography = Substitute.For<IAesCryptography>();

        var currentTime = DateTimeOffset.UtcNow;

        _ = fakeAesSystemClock.UtcNow.Returns(currentTime);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 12,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            name = playerCode,
            credit_allocated = 0
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"7602\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        await client.CreatePlayerAsync(
               apiUrl,
               domain,
               aesKey,
               aesIV,
               agent,
               playerCode
            ).ConfigureAwait(false); ;
    }
}
