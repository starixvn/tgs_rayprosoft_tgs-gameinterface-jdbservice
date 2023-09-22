using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;

public class SearchGameOldTransactionDetailsUnitTest
{
    [Fact]
    public async Task SeqNo_Normal_Process()
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
            action = 64,
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
                    ["seqNo"]= 7383541906849,
                    ["playerId"]= "4uhm33dvljda",
                    ["gType"]= 0,
                    ["mtype"]= 14065,
                    ["gameDate"]= "18-09-2023 01:38:40",
                    ["bet"]= -5.0,
                    ["win"]= 10.0,
                    ["total"]= 5.0,
                    ["currency"]= "PP",
                    ["jackpot"]= 0.0,
                    ["jackpotContribute"]= 0.0,
                    ["denom"]= 0.1,
                    ["lastModifyTime"]= "18-09-2023 01:38:40",
                    ["playerIp"]= "131.226.106.180",
                    ["clientType"]= "MOBILE",
                    ["hasFreegame"]= 0,
                    ["hasGamble"]= 0,
                    ["gambleBet"]= 0.0,
                    ["systemTakeWin"]= 0
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

        var results = await client
             .SearchGameOldTransactionDetailsAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                startTime,
                endTime)
             .ToArrayAsync()
             .ConfigureAwait(false);

        Assert.NotNull(results);
        Assert.True(results.Any());
        Assert.Equal("7383541906849", results.ElementAt(0).HistoryId);
        Assert.Equal("4uhm33dvljda", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2023-09-18T01:38:40-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(5.0m, results.ElementAt(0).Bet);
        Assert.Equal(10.0m, results.ElementAt(0).Win);
        Assert.Equal(5.0m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(0.1m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2023-09-18T01:38:40-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("131.226.106.180", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0.0m, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }

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
            action = 64,
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

        var results = await client
             .SearchGameOldTransactionDetailsAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                startTime,
                endTime)
             .ToArrayAsync()
             .ConfigureAwait(false);

        Assert.NotNull(results);
        Assert.False(results.Any());
    }

    [Fact]
    public async Task HistoryId_Normal_Process()
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
            action = 64,
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
                    ["historyId"]= 7383541906849,
                    ["playerId"]= "4uhm33dvljda",
                    ["gType"]= 0,
                    ["mtype"]= 14065,
                    ["gameDate"]= "18-09-2023 01:38:40",
                    ["bet"]= -5.0,
                    ["win"]= 10.0,
                    ["total"]= 5.0,
                    ["currency"]= "PP",
                    ["jackpot"]= 0.0,
                    ["jackpotContribute"]= 0.0,
                    ["denom"]= 0.1,
                    ["lastModifyTime"]= "18-09-2023 01:38:40",
                    ["playerIp"]= "131.226.106.180",
                    ["clientType"]= "MOBILE",
                    ["hasFreegame"]= 0,
                    ["hasGamble"]= 0,
                    ["gambleBet"]= 0.0,
                    ["systemTakeWin"]= 0
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

        var results = await client
             .SearchGameOldTransactionDetailsAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                startTime,
                endTime)
             .ToArrayAsync()
             .ConfigureAwait(false);

        Assert.NotNull(results);
        Assert.True(results.Any());
        Assert.Equal("7383541906849", results.ElementAt(0).HistoryId);
        Assert.Equal("4uhm33dvljda", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2023-09-18T01:38:40-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(5.0m, results.ElementAt(0).Bet);
        Assert.Equal(10.0m, results.ElementAt(0).Win);
        Assert.Equal(5.0m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(0.1m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2023-09-18T01:38:40-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("131.226.106.180", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
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
            action = 64,
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
                    ["seqNo"]= 7383794539225,
                    ["playerId"]= "4uk1wq8nszhn",
                    ["gameDate"]= "20-09-2023 11:49:00",
                    ["gType"]= 7,
                    ["mtype"]= 7003,
                    ["roomType"]= 0,
                    ["currency"]= "PP",
                    ["bet"]= -0.2m,
                    ["win"]= 0.0m,
                    ["total"]= -0.2m,
                    ["denom"]= 0.1,
                    ["beforeBalance"]= 70.15m,
                    ["afterBalance"]= 69.95m,
                    ["lastModifyTime"]= "20-09-2023 11:49:00",
                    ["playerIp"]= "180.190.133.140",
                    ["clientType"]= "MOBILE",
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

        var results = await client
             .SearchGameOldTransactionDetailsAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                startTime,
                endTime)
             .ToArrayAsync()
             .ConfigureAwait(false);

        Assert.NotNull(results);
        Assert.True(results.Any());
        Assert.Equal("7383794539225", results.ElementAt(0).HistoryId);
        Assert.Equal("4uk1wq8nszhn", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2023-09-20T11:49:00-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(0.2m, results.ElementAt(0).Bet);
        Assert.Equal(0.0m, results.ElementAt(0).Win);
        Assert.Equal(-0.2m, results.ElementAt(0).Total);
        Assert.Equal("PP", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(0.1m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2023-09-20T11:49:00-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("180.190.133.140", results.ElementAt(0).PlayerIp);
        Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }

    [Fact]
    public async Task Fish_HistoryId_TransactionDetails()
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
            action = 64,
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
                    ["seqNo"]= 5250214475720,
                     ["historyId"]= "5250214475720",
                    ["playerId"]= "4ufhtneqo2ia",
                    ["gameDate"]= "22-09-2023 03:39:04",
                    ["gType"]= 7,
                    ["mtype"]= 7002,
                    ["roomType"]= 1,
                    ["currency"]= "RB",
                    ["bet"]= -30.15m,
                    ["win"]= 16.8m,
                    ["total"]= -13.35m,
                    ["denom"]= 1.0m,
                    ["beforeBalance"]= 10101.15m,
                    ["afterBalance"]= 10087.8m,
                    ["lastModifyTime"]= "22-09-2023 03:39:04",
                    ["playerIp"]= "114.33.41.237",
                    ["clientType"]= "WEB",
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

        var results = await client
             .SearchGameOldTransactionDetailsAsync(
                apiUrl,
                domain,
                aesKey,
                aesIV,
                agent,
                startTime,
                endTime)
             .ToArrayAsync()
             .ConfigureAwait(false);

        Assert.NotNull(results);
        Assert.True(results.Any());
        Assert.Equal("5250214475720", results.ElementAt(0).HistoryId);
        Assert.Equal("4ufhtneqo2ia", results.ElementAt(0).PlayerId);
        Assert.Equal(DateTimeOffset.Parse("2023-09-22T03:39:04-04:00"), results.ElementAt(0).GameDate);
        Assert.Equal(30.15m, results.ElementAt(0).Bet);
        Assert.Equal(16.8m, results.ElementAt(0).Win);
        Assert.Equal(-13.35m, results.ElementAt(0).Total);
        Assert.Equal("RB", results.ElementAt(0).Currency);
        Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
        Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
        Assert.Equal(1.0m, results.ElementAt(0).Denom);
        Assert.Equal(DateTimeOffset.Parse("2023-09-22T03:39:04-04:00"), results.ElementAt(0).LastModifyTime);
        Assert.Equal("114.33.41.237", results.ElementAt(0).PlayerIp);
        Assert.Equal("WEB", results.ElementAt(0).ClientType);
        Assert.Equal(0, results.ElementAt(0).HasFreegame);
        Assert.Equal(0, results.ElementAt(0).HasGamble);
        Assert.Equal(0, results.ElementAt(0).GambleBet);
        Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
        Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
    }
}
