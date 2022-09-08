using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.GameApiClientUnitTest;

[TestClass]
public class GetTokenUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var playerCode = "4ucmqplxhfoe";
        var gameType = "7";
        var machineType = "7001";
        var lang = "cn";

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
            action = 11,
            ts = currentTime.ToUnixTimeMilliseconds(),
            uid = playerCode,
            gType = gameType,
            mType = machineType,
            lang,
            windowMode = 2,
            isAPP = false,
            moreGame = 0,
            isShowDollarSign = true
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"0000\",\"path\":\"https://localhost/\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var actual = await client.GetTokenAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                playerCode,
                gameType,
                machineType,
                lang
             ).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsPlayerExist);
        Assert.AreEqual("https://localhost/", actual.GameUrl);
    }

    [TestMethod]
    public async Task Player_Does_Not_Exist()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var playerCode = "4ucmqplxhfoe";
        var gameType = "7";
        var machineType = "7001";
        var lang = "cn";

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
            action = 11,
            ts = currentTime.ToUnixTimeMilliseconds(),
            uid = playerCode,
            gType = gameType,
            mType = machineType,
            lang,
            windowMode = 2,
            isAPP = false,
            moreGame = 0,
            isShowDollarSign = true
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"7501\",\"err_text\":\"User ID cannot be found.\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var actual = await client.GetTokenAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                playerCode,
                gameType,
                machineType,
                lang
             ).ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.IsPlayerExist);
    }
}
