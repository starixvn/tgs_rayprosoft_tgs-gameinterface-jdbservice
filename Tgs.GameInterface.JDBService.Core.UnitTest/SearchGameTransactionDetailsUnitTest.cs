using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;

public class SearchGameTransactionDetailsUnitTest
{
    [Fact]
    public async Task No_Data()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var startTime = DateTime.Parse("2023-09-18 13:35:00+08:00");
        var endTime = DateTime.Parse("2023-09-18 13:40:00+08:00");

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

        var timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        var startTimeEastern = TimeZoneInfo.ConvertTime(startTime.ToUniversalTime(), timeZone);
        var endTimeEastern = TimeZoneInfo.ConvertTime(endTime.ToUniversalTime(), timeZone);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 29,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            starttime = startTimeEastern.ToString("dd-MM-yyyy HH:mm:00"),
            endtime = endTimeEastern.ToString("dd-MM-yyyy HH:mm:00")
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");
        var respond = new JsonObject
        {
            ["status"] = "0000",
            ["data"] = new JsonArray { }
        };
        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(respond.ToJsonString()));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var results = await client.SearchGameTransactionDetailsAsync(
            apiUrl: apiUrl,
            domain: domain,
            aesKey: aesKey,
            aesIV: aesIV,
            agent: agent,
            startTime: startTime,
            endTime: endTime)
            .ToArrayAsync();

        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var startTime = DateTime.Parse("2023-09-18 13:35:00+08:00");
        var endTime = DateTime.Parse("2023-09-18 13:40:00+08:00");

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

        var timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        var startTimeEastern = TimeZoneInfo.ConvertTime(startTime.ToUniversalTime(), timeZone);
        var endTimeEastern = TimeZoneInfo.ConvertTime(endTime.ToUniversalTime(), timeZone);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 29,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            starttime = startTimeEastern.ToString("dd-MM-yyyy HH:mm:00"),
            endtime = endTimeEastern.ToString("dd-MM-yyyy HH:mm:00")
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");
        var respond = new JsonObject
        {
            ["status"] = "0000",
            ["data"] = new JsonArray {
                new JsonObject
                {
                    ["bet"]= -2.0,
                    ["win"]= 4.0,
                    ["denom"]= 2.0,
                    ["gType"]= 0,
                    ["mtype"]= 14042,
                    ["seqNo"]= 7405918749700,
                    ["total"]= 2.0,
                    ["jackpot"]= 0.0,
                    ["currency"]= "PP",
                    ["gameDate"]= "26-02-2024 22:25:14",
                    ["playerId"]= "4ujgn5ht6x7m",
                    ["playerIp"]= "175.176.24.123",
                    ["gambleBet"]= 0.0,
                    ["hasGamble"]= 0,
                    ["historyId"]= "7405918749700",
                    ["clientType"]= "MOBILE",
                    ["hasFreegame"]= 0,
                    ["systemTakeWin"]= 0,
                    ["lastModifyTime"]= "26-02-2024 22:25:14",
                    ["jackpotContribute"]= 0.0
                }
            }
        };
        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(respond.ToJsonString()));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var results = await client.SearchGameTransactionDetailsAsync(
            apiUrl: apiUrl,
            domain: domain,
            aesKey: aesKey,
            aesIV: aesIV,
            agent: agent,
            startTime: startTime,
            endTime: endTime)
            .ToArrayAsync();

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal("7405918749700", results.ElementAt(0).RoundSeqNo);
        Assert.Equal("7405918749700", results.ElementAt(0).HistoryId);
        Assert.Equal("4ujgn5ht6x7m", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2024-02-26T22:25:14-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(2.0m, results.ElementAt(0).Bet);
        Assert.Equal(4.0m, results.ElementAt(0).Win);
        Assert.Equal(2.0m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(2.0m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2024-02-26T22:25:14-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("175.176.24.123", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0.0m, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }

    [Fact]
    public async Task RoundSeqNo_Normal_Process()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var startTime = DateTime.Parse("2023-09-18 13:35:00+08:00");
        var endTime = DateTime.Parse("2023-09-18 13:40:00+08:00");

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

        var timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        var startTimeEastern = TimeZoneInfo.ConvertTime(startTime.ToUniversalTime(), timeZone);
        var endTimeEastern = TimeZoneInfo.ConvertTime(endTime.ToUniversalTime(), timeZone);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 29,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            starttime = startTimeEastern.ToString("dd-MM-yyyy HH:mm:00"),
            endtime = endTimeEastern.ToString("dd-MM-yyyy HH:mm:00")
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");
        var respond = new JsonObject
        {
            ["status"] = "0000",
            ["data"] = new JsonArray {
                new JsonObject
                {
                    ["roundSeqNo"]= 7405918749700,
                    ["bet"]= -2.0,
                    ["win"]= 4.0,
                    ["denom"]= 2.0,
                    ["gType"]= 0,
                    ["mtype"]= 14042,
                    ["seqNo"]= 7405918749700,
                    ["total"]= 2.0,
                    ["jackpot"]= 0.0,
                    ["currency"]= "PP",
                    ["gameDate"]= "26-02-2024 22:25:14",
                    ["playerId"]= "4ujgn5ht6x7m",
                    ["playerIp"]= "175.176.24.123",
                    ["gambleBet"]= 0.0,
                    ["hasGamble"]= 0,
                    ["historyId"]= "7405918749700",
                    ["clientType"]= "MOBILE",
                    ["hasFreegame"]= 0,
                    ["systemTakeWin"]= 0,
                    ["lastModifyTime"]= "26-02-2024 22:25:14",
                    ["jackpotContribute"]= 0.0
                }
            }
        };
        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(respond.ToJsonString()));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var results = await client.SearchGameTransactionDetailsAsync(
            apiUrl: apiUrl,
            domain: domain,
            aesKey: aesKey,
            aesIV: aesIV,
            agent: agent,
            startTime: startTime,
            endTime: endTime)
            .ToArrayAsync();

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal("7405918749700", results.ElementAt(0).RoundSeqNo);
        Assert.Equal("7405918749700", results.ElementAt(0).HistoryId);
        Assert.Equal("4ujgn5ht6x7m", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2024-02-26T22:25:14-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(2.0m, results.ElementAt(0).Bet);
        Assert.Equal(4.0m, results.ElementAt(0).Win);
        Assert.Equal(2.0m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(2.0m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2024-02-26T22:25:14-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("175.176.24.123", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0.0m, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }

    [Fact]
    public async Task Freegame()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var startTime = DateTime.Parse("2023-09-19 11:30:00+08:00");
        var endTime = DateTime.Parse("2023-09-19 11:40:00+08:00");

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

        var timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        var startTimeEastern = TimeZoneInfo.ConvertTime(startTime.ToUniversalTime(), timeZone);
        var endTimeEastern = TimeZoneInfo.ConvertTime(endTime.ToUniversalTime(), timeZone);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 29,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            starttime = startTimeEastern.ToString("dd-MM-yyyy HH:mm:00"),
            endtime = endTimeEastern.ToString("dd-MM-yyyy HH:mm:00")
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");
        var respond = new JsonObject
        {
            ["status"] = "0000",
            ["data"] = new JsonArray {
                new JsonObject
                {
                    ["bet"]= -20.0,
                    ["win"]= 520.0,
                    ["denom"]= 2.0,
                    ["gType"]= 0,
                    ["mtype"]= 14042,
                    ["seqNo"]= 7405912472192,
                    ["total"]= 500.0,
                    ["jackpot"]= 0.0,
                    ["currency"]= "PP",
                    ["gameDate"]= "26-02-2024 21:34:49",
                    ["playerId"]= "4ume6i0bb37x",
                    ["playerIp"]= "122.54.227.234",
                    ["gambleBet"]= 0.0,
                    ["hasGamble"]= 0,
                    ["historyId"]= "7405912472192",
                    ["clientType"]= "MOBILE",
                    ["hasFreegame"]= 1,
                    ["systemTakeWin"]= 0,
                    ["lastModifyTime"]= "26-02-2024 21:34:49",
                    ["jackpotContribute"]= 0.0
                }
            }
        };
        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(respond.ToJsonString()));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var results = await client.SearchGameTransactionDetailsAsync(
            apiUrl: apiUrl,
            domain: domain,
            aesKey: aesKey,
            aesIV: aesIV,
            agent: agent,
            startTime: startTime,
            endTime: endTime)
            .ToArrayAsync();

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal("7405912472192", results.ElementAt(0).RoundSeqNo);
        Assert.Equal("7405912472192", results.ElementAt(0).HistoryId);
        Assert.Equal("4ume6i0bb37x", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2024-02-26T21:34:49-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(20.0m, results.ElementAt(0).Bet);
        Assert.Equal(520.0m, results.ElementAt(0).Win);
        Assert.Equal(500.0m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(2.0m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2024-02-26T21:34:49-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("122.54.227.234", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(1, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0.0m, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }

    [Fact]
    public async Task Fish_TransactionDetails()
    {
        var apiUrl = new Uri("http://localhost/");
        var domain = "ZF";
        var aesKey = "831057181dd07807";
        var aesIV = "193a3159afebee65";
        var agent = "zf055gpi";
        var startTime = DateTime.Parse("2023-09-18 13:35:00+08:00");
        var endTime = DateTime.Parse("2023-09-18 13:40:00+08:00");

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

        var timeZone = TimeZoneInfo.CreateCustomTimeZone(id: "eastern time", baseUtcOffset: TimeSpan.FromHours(-4), "eastern time", "eastern time");
        var startTimeEastern = TimeZoneInfo.ConvertTime(startTime.ToUniversalTime(), timeZone);
        var endTimeEastern = TimeZoneInfo.ConvertTime(endTime.ToUniversalTime(), timeZone);

        var encryptedData = Guid.NewGuid().ToString("N");
        var data = new
        {
            action = 29,
            ts = currentTime.ToUnixTimeMilliseconds(),
            parent = agent,
            starttime = startTimeEastern.ToString("dd-MM-yyyy HH:mm:00"),
            endtime = endTimeEastern.ToString("dd-MM-yyyy HH:mm:00")
        };
        var jsonString = JsonSerializer.Serialize(data);
        _ = fakeAesCryptography.GetAesEncryptAsync(
            Arg.Is(jsonString),
            Arg.Is(aesKey),
            Arg.Is(aesIV)).Returns(encryptedData);

        var url = new Uri(apiUrl, "/apiRequest.do");
        var respond = new JsonObject
        {
            ["status"] = "0000",
            ["data"] = new JsonArray {
                new JsonObject
                {
                    ["bet"]= -12.0m,
                    ["win"]= 9.5m,
                    ["denom"]= 1.0m,
                    ["gType"]= 7,
                    ["mtype"]= 7003,
                    ["seqNo"]= 7405690178678,
                    ["total"]= -2.5m,
                    ["currency"]= "PP",
                    ["gameDate"]= "25-02-2024 13:44:21",
                    ["playerId"]= "4uicun5ssgu6",
                    ["playerIp"]= "112.198.114.77",
                    ["roomType"]= 0,
                    ["historyId"]= "7405690178678",
                    ["clientType"]= "MOBILE",
                    ["afterBalance"]= 1.3m,
                    ["beforeBalance"]= 3.8m,
                    ["lastModifyTime"]= "25-02-2024 13:44:21"
                }
            }
        };
        var handler = new MockHttpMessageHandler();
        _ = handler
            .Expect(HttpMethod.Post, url.ToString())
            .Respond(HttpStatusCode.OK, new StringContent(respond.ToJsonString()));

        var fakeHttpClient = handler.ToHttpClient();

        var client = new GameApiClient(
            gameInterfaceSetting,
            fakeLog,
            fakeHttpClient,
            fakeAesSystemClock,
            fakeAesCryptography);

        var results = await client.SearchGameTransactionDetailsAsync(
            apiUrl: apiUrl,
            domain: domain,
            aesKey: aesKey,
            aesIV: aesIV,
            agent: agent,
            startTime: startTime,
            endTime: endTime)
            .ToArrayAsync();

        Assert.NotNull(results);
        Assert.NotEmpty(results);
        Assert.Equal("7405690178678", results.ElementAt(0).RoundSeqNo);
        Assert.Equal("7405690178678", results.ElementAt(0).HistoryId);
        Assert.Equal("4uicun5ssgu6", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2024-02-25T13:44:21-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(12.0m, results.ElementAt(0).Bet);
        Assert.Equal(9.5m, results.ElementAt(0).Win);
        Assert.Equal(-2.5m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(1.0m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2024-02-25T13:44:21-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("112.198.114.77", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0.0m, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }
}
