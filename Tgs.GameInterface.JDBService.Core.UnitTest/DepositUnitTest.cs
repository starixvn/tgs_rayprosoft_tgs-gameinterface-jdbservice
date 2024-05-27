using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.JDBService.Core.Exceptions;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;

public class DepositUnitTest
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
		var serialNo = "1662442853";
		var amount = 1m;

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
			action = 19,
			ts = currentTime.ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode,
			serialNo,
			allCashOutFlag = 0,
			amount
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
			["userBalance"] = 1.0m,
			["agentCashBalance"] = 0.0m,
			["amount"] = 1.0m,
			["pid"] = 4960023,
			["payDate"] = "06-09-2022 04:16:39",
			["serialNo"] = "1662442853",
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

		var actual = await client.DepositAsync(
			apiUrl: apiUrl,
			domain: domain,
			aesKey: aesKey,
			aesIV: aesIV,
			agent: agent,
			playerCode: playerCode,
			serialNo: serialNo,
			amount: amount);

		Assert.NotNull(actual);
		Assert.Equal("1662442853", actual.SerialNo);
		Assert.Equal(respond.ToJsonString(), actual.Resp);
	}

	[Fact]
	public async Task Status_6009_Exception()
	{
		var apiUrl = new Uri("http://localhost/");
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var encryptedData = Guid.NewGuid().ToString("N");
		var data = new
		{
			action = 19,
			ts = currentTime.ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode,
			serialNo,
			allCashOutFlag = 0,
			amount
		};
		var jsonString = JsonSerializer.Serialize(data);
		_ = fakeAesCryptography.GetAesEncryptAsync(
			Arg.Is(jsonString),
			Arg.Is(aesKey),
			Arg.Is(aesIV)).Returns(encryptedData);

		var url = new Uri(apiUrl, "/apiRequest.do");
		var respond = new JsonObject
		{
			["status"] = "6009",
			["err_text"] = "The deposit amount exceeds the upper limit."
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

		_ = await Assert.ThrowsAsync<Status_6009_Exception>(
			async () => await client.DepositAsync(
				apiUrl: apiUrl,
				domain: domain,
				aesKey: aesKey,
				aesIV: aesIV,
				agent: agent,
				playerCode: playerCode,
				serialNo: serialNo,
				amount: amount));
	}

	[Fact]
	public async Task Status_6010_Exception()
	{
		var apiUrl = new Uri("http://localhost/");
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
		var fakeTimeProvider = Substitute.For<TimeProvider>();
		var fakeAesCryptography = Substitute.For<IAesCryptography>();

		var currentTime = DateTimeOffset.UtcNow;

		_ = fakeTimeProvider.GetUtcNow().Returns(currentTime);

		var encryptedData = Guid.NewGuid().ToString("N");
		var data = new
		{
			action = 19,
			ts = currentTime.ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode,
			serialNo,
			allCashOutFlag = 0,
			amount
		};
		var jsonString = JsonSerializer.Serialize(data);
		_ = fakeAesCryptography.GetAesEncryptAsync(
			Arg.Is(jsonString),
			Arg.Is(aesKey),
			Arg.Is(aesIV)).Returns(encryptedData);

		var url = new Uri(apiUrl, "/apiRequest.do");
		var respond = new JsonObject
		{
			["status"] = "6010",
			["err_text"] = "Balance exceeds the upper limit."
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

		_ = await Assert.ThrowsAsync<Status_6010_Exception>(
			async () => await client.DepositAsync(
				apiUrl: apiUrl,
				domain: domain,
				aesKey: aesKey,
				aesIV: aesIV,
				agent: agent,
				playerCode: playerCode,
				serialNo: serialNo,
				amount: amount));
	}
}
