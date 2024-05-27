using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.JDBService.Protos;

namespace Tgs.GameInterface.JDBService.Client;

internal class GameInterfaceJDBClient(
	GameInterfaceJDBService.GameInterfaceJDBServiceClient client) : IGameInterfaceJDBClient
{
	public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
	{
		var request = new CheckingServiceRequest
		{
			MachineName = machineName,
		};

		var reply = await client
			.CheckingServiceAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.MachineName;
	}

	public async ValueTask<FundReply> FundInAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, string serialNo, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode,
			SerialNo = serialNo,
			Amount = amount
		};
		return await client
			.FundInAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, string serialNo, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode,
			SerialNo = serialNo,
			Amount = amount
		};
		return await client
			.FundOutAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> FundStatusAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string serialNo, CancellationToken cancellationToken = default)
	{
		var request = new FundStatusRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			SerialNo = serialNo
		};
		var reply = await client
			.FundStatusAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.IsOrderExist;
	}

	public async ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string playerCode, string gameType, string machineType, string lang, CancellationToken cancellationToken = default)
	{
		var request = new GameUrlRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			PlayerCode = playerCode,
			GameType = gameType,
			MachineType = machineType,
			Lang = lang
		};
		return await client
			.GameUrlAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async IAsyncEnumerable<GameWagerInfo> GameWagerAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameWagerRequest
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp()
		};
		var reply = await client
			.GameWagerAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (reply.Items.Count == 0)
		{
			yield break;
		}

		foreach (var item in reply.Items ?? Enumerable.Empty<GameWagerInfo>())
		{
			yield return item;
		}
	}

	public async IAsyncEnumerable<GameWagerInfo> GameWagerOldAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameWagerRequest
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp()
		};
		var reply = await client
			.GameWagerOldAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (reply.Items.Count == 0)
		{
			yield break;
		}

		foreach (var item in reply.Items ?? Enumerable.Empty<GameWagerInfo>())
		{
			yield return item;
		}
	}

	public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerBalanceRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode
		};
		return await client
			.PlayerBalanceAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask PlayerCreateAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerCreateRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode
		};
		_ = await client
			.PlayerCreateAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask PlayerKickAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerKickRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode
		};
		_ = await client
			.PlayerKickAsync(
				request: request,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask PlayerLogoutAllAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, CancellationToken cancellationToken = default)
	{
		var request = new PlayerLogoutAllRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent
		};
		_ = await client.PlayerLogoutAllAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask PlayerLogoutAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerLogoutRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode
		};
		_ = await client.PlayerLogoutAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerOnlineRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Domain = domain,
			AesKey = aesKey,
			AesIV = aesIV,
			Agent = agent,
			PlayerCode = playerCode
		};
		var reply = await client.PlayerOnlineAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.Online;
	}
}
