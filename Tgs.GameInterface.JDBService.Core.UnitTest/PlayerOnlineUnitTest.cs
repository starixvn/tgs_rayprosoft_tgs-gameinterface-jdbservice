using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;

public class PlayerOnlineUnitTest
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var encryptedData = Guid.NewGuid().ToString("N");
		var data = new
		{
			action = 52,
			ts = currentTime.ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode
		};
		var jsonString = JsonSerializer.Serialize(data);
		_ = fakeAesCryptography.GetAesEncryptAsync(
			Arg.Is(jsonString),
			Arg.Is(aesKey),
			Arg.Is(aesIV)).Returns(encryptedData);

		var url = new Uri(apiUrl, "/apiRequest.do");

		var response = new JsonObject
		{
			["status"] = "0000",
			["data"] = new JsonObject
			{
				["gType"] = 0,
				["mType"] = 8011,
				["loginFrom"] = "WEB",
				["ipAddr"] = "10.20.100.13",
				["loginTime"] = "06-11-2017 16:49:18",
				["balance"] = 1000
			}
		};

		var handler = new MockHttpMessageHandler();
		_ = handler
			.Expect(HttpMethod.Post, url.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString()));

		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			gameInterfaceSetting,
			fakeLog,
			fakeHttpClient,
			fakeTimeProvider,
			fakeAesCryptography);

		var actual = await client.PlayerOnlineAsync(
			apiUrl: apiUrl,
			domain: domain,
			aesKey: aesKey,
			aesIV: aesIV,
			agent: agent,
			playerCode: playerCode);

		Assert.True(actual);
	}

	[Fact]
	public async Task User_is_not_playing()
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var encryptedData = Guid.NewGuid().ToString("N");
		var data = new
		{
			action = 52,
			ts = currentTime.ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode
		};
		var jsonString = JsonSerializer.Serialize(data);
		_ = fakeAesCryptography.GetAesEncryptAsync(
			Arg.Is(jsonString),
			Arg.Is(aesKey),
			Arg.Is(aesIV)).Returns(encryptedData);

		var url = new Uri(apiUrl, "/apiRequest.do");

		var response = new JsonObject
		{
			["status"] = "7505",
			["err_text"] = "User is not playing",
		};

		var handler = new MockHttpMessageHandler();
		_ = handler
			.Expect(HttpMethod.Post, url.ToString())
			.Respond(HttpStatusCode.OK, new StringContent(response.ToJsonString()));

		var fakeHttpClient = handler.ToHttpClient();

		var client = new GameApiClient(
			gameInterfaceSetting,
			fakeLog,
			fakeHttpClient,
			fakeTimeProvider,
			fakeAesCryptography);

		var actual = await client.PlayerOnlineAsync(
			apiUrl: apiUrl,
			domain: domain,
			aesKey: aesKey,
			aesIV: aesIV,
			agent: agent,
			playerCode: playerCode);

		Assert.False(actual);
	}
}
