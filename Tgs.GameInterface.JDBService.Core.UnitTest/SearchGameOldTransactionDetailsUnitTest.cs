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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

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
			fakeTimeProvider,
			fakeAesCryptography);

		var results = await client.SearchGameOldTransactionDetailsAsync(
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

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
					["bet"]= -0.25,
					["win"]= 1.25,
					["denom"]= 0.01,
					["gType"]= 0,
					["mtype"]= 8020,
					["seqNo"]= 7410025380879,
					["total"]= 1.0,
					["jackpot"]= 0.0,
					["currency"]= "PP",
					["gameDate"]= "19-03-2024 23:15:47",
					["playerId"]= "4ugyisoukd9p",
					["playerIp"]= "136.158.3.190",
					["historyId"]= "7410025380879",
					["clientType"]= "MOBILE",
					["hasFreegame"]= 0,
					["afterBalance"]= "1.64",
					["beforeBalance"]= "0.64",
					["systemTakeWin"]= 0,
					["lastModifyTime"]= "19-03-2024 23:15:47",
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
			fakeTimeProvider,
			fakeAesCryptography);

		var results = await client.SearchGameOldTransactionDetailsAsync(
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
		Assert.Equal("7410025380879", results.ElementAt(0).RoundSeqNo);
		Assert.Equal("7410025380879", results.ElementAt(0).HistoryId);
		Assert.Equal("4ugyisoukd9p", results.ElementAt(0).PlayerId);
		Assert.Equal(DateTimeOffset.Parse("2024-03-19T23:15:47-04:00"), results.ElementAt(0).GameDate);
		Assert.Equal(0.25m, results.ElementAt(0).Bet);
		Assert.Equal(1.25m, results.ElementAt(0).Win);
		Assert.Equal(1.0m, results.ElementAt(0).Total);
		Assert.Equal("PP", results.ElementAt(0).Currency);
		Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
		Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
		Assert.Equal(0.01m, results.ElementAt(0).Denom);
		Assert.Equal(DateTimeOffset.Parse("2024-03-19T23:15:47-04:00"), results.ElementAt(0).LastModifyTime);
		Assert.Equal("136.158.3.190", results.ElementAt(0).PlayerIp);
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

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
			fakeTimeProvider,
			fakeAesCryptography);

		var results = await client.SearchGameOldTransactionDetailsAsync(
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
		var startTime = DateTime.Parse("2023-09-18 13:35:00+08:00");
		var endTime = DateTime.Parse("2023-09-18 13:40:00+08:00");

		var gameInterfaceSetting = Options.Create(new GameInterfaceSettingOptions()
		{
			WithdrawalRetryCount = 1,
			WithdrawalIntervalSeconds = 1
		});
		var fakeLog = NullLogger<GameApiClient>.Instance;
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

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
					["bet"]= -1.2,
					["win"]= 12.48,
					["denom"]= 0.02,
					["gType"]= 0,
					["mtype"]= 14077,
					["seqNo"]= 7410025192659,
					["total"]= 11.28,
					["jackpot"]= 0.0,
					["currency"]= "HK",
					["gameDate"]= "19-03-2024 23:14:22",
					["playerId"]= "4ukazt29act5",
					["playerIp"]= "116.48.51.172",
					["historyId"]= "7410025192659",
					["clientType"]= "WEB",
					["hasFreegame"]= 1,
					["afterBalance"]= "152.52",
					["beforeBalance"]= "141.24",
					["systemTakeWin"]= 0,
					["lastModifyTime"]= "19-03-2024 23:14:22",
					["jackpotContribute"]=  0.0,
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
			fakeTimeProvider,
			fakeAesCryptography);

		var results = await client.SearchGameOldTransactionDetailsAsync(
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
		Assert.Equal("7410025192659", results.ElementAt(0).RoundSeqNo);
		Assert.Equal("7410025192659", results.ElementAt(0).HistoryId);
		Assert.Equal("4ukazt29act5", results.ElementAt(0).PlayerId);
		Assert.Equal(DateTimeOffset.Parse("2024-03-19T23:14:22-04:00"), results.ElementAt(0).GameDate);
		Assert.Equal(1.2m, results.ElementAt(0).Bet);
		Assert.Equal(12.48m, results.ElementAt(0).Win);
		Assert.Equal(11.28m, results.ElementAt(0).Total);
		Assert.Equal("HK", results.ElementAt(0).Currency);
		Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
		Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
		Assert.Equal(0.02m, results.ElementAt(0).Denom);
		Assert.Equal(DateTimeOffset.Parse("2024-03-19T23:14:22-04:00"), results.ElementAt(0).LastModifyTime);
		Assert.Equal("116.48.51.172", results.ElementAt(0).PlayerIp);
		Assert.Equal("WEB", results.ElementAt(0).ClientType);
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

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
					["bet"]= -20.1m,
					["win"]= 9.9m,
					["denom"]= 1.0m,
					["gType"]= 7,
					["mtype"]= 7006,
					["seqNo"]= 7409759453101,
					["total"]= -10.2m,
					["currency"]= "VN",
					["gameDate"]= "18-03-2024 14:15:38",
					["playerId"]= "4umj9i07xvdf",
					["playerIp"]= "27.74.101.2",
					["roomType"]= 0,
					["historyId"]= "7409759453101",
					["clientType"]= "MOBILE",
					["afterBalance"]= 0.1m,
					["beforeBalance"]= 10.3m,
					["lastModifyTime"]= "18-03-2024 14:15:38"
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
			fakeTimeProvider,
			fakeAesCryptography);

		var results = await client.SearchGameOldTransactionDetailsAsync(
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
		Assert.Equal("7409759453101", results.ElementAt(0).RoundSeqNo);
		Assert.Equal("7409759453101", results.ElementAt(0).HistoryId);
		Assert.Equal("4umj9i07xvdf", results.ElementAt(0).PlayerId);
		Assert.Equal(DateTimeOffset.Parse("2024-03-18T14:15:38-04:00"), results.ElementAt(0).GameDate);
		Assert.Equal(20.1m, results.ElementAt(0).Bet);
		Assert.Equal(9.9m, results.ElementAt(0).Win);
		Assert.Equal(-10.2m, results.ElementAt(0).Total);
		Assert.Equal("VN", results.ElementAt(0).Currency);
		Assert.Equal(0.0m, results.ElementAt(0).Jackpot);
		Assert.Equal(0.0m, results.ElementAt(0).JackpotContribute);
		Assert.Equal(1.0m, results.ElementAt(0).Denom);
		Assert.Equal(DateTimeOffset.Parse("2024-03-18T14:15:38-04:00"), results.ElementAt(0).LastModifyTime);
		Assert.Equal("27.74.101.2", results.ElementAt(0).PlayerIp);
		Assert.Equal("MOBILE", results.ElementAt(0).ClientType);
		Assert.Equal(0, results.ElementAt(0).HasFreegame);
		Assert.Equal(0, results.ElementAt(0).HasGamble);
		Assert.Equal(0.0m, results.ElementAt(0).GambleBet);
		Assert.Equal(0, results.ElementAt(0).SystemTakeWin);
		Assert.Equal(respond["data"]!.AsArray()!.ElementAt(0)!.ToJsonString(), results.ElementAt(0).AdditionalData);
	}
}
