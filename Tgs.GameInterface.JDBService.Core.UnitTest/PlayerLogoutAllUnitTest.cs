using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.JDBService.Core.UnitTest;
public class PlayerLogoutAllUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("http://localhost/");
		var domain = "ZF";
		var aesKey = "831057181dd07807";
		var aesIV = "193a3159afebee65";
		var agent = "zf055gpi";

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
			action = 58,
			ts = currentTime.ToUnixTimeMilliseconds(),
			parent = agent
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

		await client.PlayerLogoutAllAsync(
			apiUrl: apiUrl,
			domain: domain,
			aesKey: aesKey,
			aesIV: aesIV,
			agent: agent);
	}
}
