using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.JDBService.Core.Exceptions;

namespace Tgs.GameInterface.JDBService.Core.UnitTest.GameApiClientUnitTest;

[TestClass]
public class WithdrawalUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ucmqplxhfoe";
        var serialNo = "1662442853";
        var amount = 1m;

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
            action = 19,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            serialNo,
            allCashOutFlag = 0,
            amount = -amount
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"0000\",\"userBalance\":1.0,\"agentCashBalance\":0.0,\"amount\":-1.0,\"pid\":4960028,\"payDate\":\"06-09-2022 04:21:37\",\"serialNo\":\"1662452479\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var actual = await client
             .WithdrawalAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                playerCode,
                serialNo,
                amount)
             .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(1m, actual.UserBalance);
        Assert.AreEqual(-1m, actual.Amount);
    }

    [TestMethod]
    public async Task Status_6009_Exception()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ucmqplxhfoe";
        var serialNo = "1662442853";
        var amount = (decimal)Math.Pow(10, 10);

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
            action = 19,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            serialNo,
            allCashOutFlag = 0,
            amount = -amount
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"6009\",\"err_text\":\"The deposit amount exceeds the upper limit.\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        _ = await Assert.ThrowsExceptionAsync<Status_6009_Exception>(async () => await client
              .WithdrawalAsync(
                 apiUrl,
                 domain,
                 aesKey,
                 aesIV,
                 agent,
                 playerCode,
                 serialNo,
                 amount)
              .ConfigureAwait(false));
    }

    [TestMethod]
    public async Task Status_6010_Exception()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ucmqplxhfoe";
        var serialNo = "1662442853";
        var amount = (decimal)Math.Pow(10, 10);

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
            action = 19,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            serialNo,
            allCashOutFlag = 0,
            amount = -amount
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"6010\",\"err_text\":\"Balance exceeds the upper limit.\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        _ = await Assert.ThrowsExceptionAsync<Status_6010_Exception>(async () => await client
              .WithdrawalAsync(
                 apiUrl,
                 domain,
                 aesKey,
                 aesIV,
                 agent,
                 playerCode,
                 serialNo,
                 amount)
              .ConfigureAwait(false));
    }

    [TestMethod]
    public async Task Status_6901_User_Is_Playing_Game_And_Not_Allow_Transfer_Balance()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ucmqplxhfoe";
        var serialNo = "1662442853";
        var amount = (decimal)Math.Pow(10, 10);

        var gameInterfaceSetting = Options.Create(new GameInterfaceSettingOptions()
        {
            WithdrawalRetryCount = 1,
            WithdrawalIntervalSeconds = 0.1
        });
        var fakeLog = NullLogger<GameApiClient>.Instance;
        var fakeAesSystemClock = Substitute.For<ISystemClock>();
        var fakeAesCryptography = Substitute.For<IAesCryptography>();

        var currentTime = DateTimeOffset.UtcNow;

        _ = fakeAesSystemClock.UtcNow.Returns(currentTime);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 19,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            serialNo,
            allCashOutFlag = 0,
            amount = -amount
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"6901\",\"err_text\":\"User is playing game, and not allow transfer balance.\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        _ = await Assert.ThrowsExceptionAsync<HttpRequestException>(async () => await client
              .WithdrawalAsync(
                 apiUrl,
                 domain,
                 aesKey,
                 aesIV,
                 agent,
                 playerCode,
                 serialNo,
                 amount)
              .ConfigureAwait(false));
    }

    [TestMethod]
    public async Task Normal_Process_DataTime()
    {
        var apiUrl = new Uri("http://api.jygrq.com");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var playerCode = "4ued5l50heuf";
        var serialNo = "357d8ba78301000000010000244ec6f1";
        var amount = 1000m;

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
            action = 19,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            serialNo,
            allCashOutFlag = 0,
            amount = -amount
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");

        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent("{\"status\":\"0000\",\"userBalance\":0.33,\"agentCashBalance\":0.0,\"amount\":-1000.0,\"pid\":5042256,\"payDate\":\"13-09-2022 02:16:03\",\"serialNo\":\"357d8ba78301000000010000244ec6f1\"}"));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var actual = await client
             .WithdrawalAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                playerCode,
                serialNo,
                amount)
             .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.AreEqual(0.33m, actual.UserBalance);
        Assert.AreEqual(-1000m, actual.Amount);
        Assert.AreEqual(DateTimeOffset.Parse("2022-09-13 02:16:03 -04:00"), actual.PayDate);
    }
}
