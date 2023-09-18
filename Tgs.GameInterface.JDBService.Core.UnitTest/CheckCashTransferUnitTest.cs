using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;

public class CheckCashTransferUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var serialNo = "1662442853";

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
            action = 55,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            serialNo
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"0000\",\"data\":[{\"uid\":\"4ucmqplxhfoe\",\"action\":\"deposit\",\"amount\":1,\"plsBalance\":1,\"date\":\"06-09-2022 04:20:47\",\"pid\":4960027,\"remark\":\"\",\"afterBalance\":2}]}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var actual = await client
             .CheckCashTransferAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                serialNo)
             .ConfigureAwait(false);

        Assert.True(actual);
    }

    [Fact]
    public async Task Status_9015_Exception()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var serialNo = "1662442853";

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
            action = 55,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            serialNo
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"9015\",\"err_text\":\"Data does not exist.\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var actual = await client
             .CheckCashTransferAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                serialNo)
             .ConfigureAwait(false);

        Assert.False(actual);
    }
}
