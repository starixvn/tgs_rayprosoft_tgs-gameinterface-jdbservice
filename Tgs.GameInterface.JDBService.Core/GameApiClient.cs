using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameInterface.JDBService.Core.Exceptions;
using Tgs.GameInterface.JDBService.Core.JsonConverters;
using Tgs.GameInterface.JDBService.Core.Models;

namespace Tgs.GameInterface.JDBService.Core;

internal class GameApiClient(
	IOptions<GameInterfaceSettingOptions> options,
	ILogger<GameApiClient> logger,
	HttpClient httpClient,
	TimeProvider timeProvider,
	IAesCryptography aesCryptography) : IGameApiClient
{
	private readonly GameInterfaceSettingOptions _gameInterfaceSetting = options.Value;

	private readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
		DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
		Converters = { new JDBDateTimeConverter() }
	};

	public async ValueTask<bool> CheckCashTransferAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string serialNo, CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 55,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			serialNo
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
					apiUrl,
					domain,
					encryptedData,
					"CheckTransactions",
					cancellationToken)
				.ConfigureAwait(false);

		return jsonNode["status"]!.ToString().Equals("0000", StringComparison.OrdinalIgnoreCase);
	}

	public async ValueTask CreatePlayerAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 12,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode,
			name = playerCode,
			credit_allocated = 0
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
				apiUrl,
				domain,
				encryptedData,
				"GetPlayerData",
				cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		if (status is not "7602" and not "0000")
		{
			throw new GameProvisioningErrorException(status, errText ?? string.Empty);
		}
	}

	public async ValueTask<TransactionResp> DepositAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, string serialNo, decimal amount, CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 19,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode,
			serialNo,
			allCashOutFlag = 0,
			amount
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
					apiUrl,
					domain,
					encryptedData,
					"Deposit",
					cancellationToken)
				.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();

		return status.Equals("6009", StringComparison.OrdinalIgnoreCase)
			? throw new Status_6009_Exception(amount)
			: status.Equals("6010", StringComparison.OrdinalIgnoreCase)
			? throw new Status_6010_Exception(amount)
			: !status.Equals("0000", StringComparison.OrdinalIgnoreCase)
			? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
			: new TransactionResp(
				SerialNo: jsonNode["serialNo"]!.ToString(),
				Resp: jsonNode.ToJsonString());
	}

	public async IAsyncEnumerable<GameDetailTransactionResp> SearchGameTransactionDetailsAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 29,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			starttime = startTime.DatetimeConvertToString(),
			endtime = endTime.DatetimeConvertToString()
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
				apiUrl,
				domain,
				encryptedData,
				"SearchGameTransactionDetails",
				cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		if (status is not "0000" and not "8006")
		{
			throw new GameProvisioningErrorException(status, errText ?? string.Empty);
		}

		var transactions = jsonNode["data"]!.AsArray();

		if (transactions.Count == 0)
		{
			yield break;
		}

		foreach (var transaction in transactions)
		{
			if (transaction is null)
			{
				continue;
			}

			yield return new GameDetailTransactionResp(
				RoundSeqNo: transaction["roundSeqNo"]?.ToString() ?? transaction["historyId"]?.ToString() ?? transaction["seqNo"]!.ToString(),
				HistoryId: transaction["historyId"]?.ToString() ?? transaction["seqNo"]!.ToString(),
				PlayerId: transaction["playerId"]!.ToString(),
				GameType: transaction["gType"]!.GetValue<int>(),
				MachineType: transaction["mtype"]!.GetValue<int>(),
				GameDate: transaction["gameDate"]!.ToString().StringConvertToDatetime(),
				Bet: -transaction["bet"]!.GetValue<decimal>(),
				Win: transaction["win"]!.GetValue<decimal>(),
				Total: transaction["total"]!.GetValue<decimal>(),
				Currency: transaction["currency"]!.ToString(),
				Jackpot: transaction["jackpot"]?.GetValue<decimal>() ?? 0,
				JackpotContribute: transaction["jackpotContribute"]?.GetValue<decimal>() ?? 0,
				Denom: transaction["denom"]!.GetValue<decimal>(),
				LastModifyTime: transaction["lastModifyTime"]!.ToString().StringConvertToDatetime(),
				PlayerIp: transaction["playerIp"]!.ToString(),
				ClientType: transaction["clientType"]!.ToString(),
				HasFreegame: transaction["hasFreegame"]?.GetValue<int>() ?? 0,
				HasGamble: transaction["hasGamble"]?.GetValue<int>() ?? 0,
				GambleBet: -transaction["gambleBet"]?.GetValue<decimal>() ?? 0.0m,
				SystemTakeWin: transaction["systemTakeWin"]?.GetValue<int>() ?? 0,
				AdditionalData: transaction.ToJsonString());
		}
	}

	public async IAsyncEnumerable<GameDetailTransactionResp> SearchGameOldTransactionDetailsAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 64,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			starttime = startTime.DatetimeConvertToString(),
			endtime = endTime.DatetimeConvertToString()
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
				apiUrl,
				domain,
				encryptedData,
				"SearchGameOldTransactionDetails",
				cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		if (status is not "0000" and not "8006")
		{
			throw new GameProvisioningErrorException(status, errText ?? string.Empty);
		}

		var transactions = jsonNode["data"]!.AsArray();

		if (transactions.Count == 0)
		{
			yield break;
		}

		foreach (var transaction in transactions)
		{
			if (transaction is null)
			{
				continue;
			}

			yield return new GameDetailTransactionResp(
				RoundSeqNo: transaction["roundSeqNo"]?.ToString() ?? transaction["historyId"]?.ToString() ?? transaction["seqNo"]!.ToString(),
				HistoryId: transaction["historyId"]?.ToString() ?? transaction["seqNo"]!.ToString(),
				PlayerId: transaction["playerId"]!.ToString(),
				GameType: transaction["gType"]!.GetValue<int>(),
				MachineType: transaction["mtype"]!.GetValue<int>(),
				GameDate: transaction["gameDate"]!.ToString().StringConvertToDatetime(),
				Bet: -transaction["bet"]!.GetValue<decimal>(),
				Win: transaction["win"]!.GetValue<decimal>(),
				Total: transaction["total"]!.GetValue<decimal>(),
				Currency: transaction["currency"]!.ToString(),
				Jackpot: transaction["jackpot"]?.GetValue<decimal>() ?? 0,
				JackpotContribute: transaction["jackpotContribute"]?.GetValue<decimal>() ?? 0,
				Denom: transaction["denom"]!.GetValue<decimal>(),
				LastModifyTime: transaction["lastModifyTime"]!.ToString().StringConvertToDatetime(),
				PlayerIp: transaction["playerIp"]!.ToString(),
				ClientType: transaction["clientType"]!.ToString(),
				HasFreegame: transaction["hasFreegame"]?.GetValue<int>() ?? 0,
				HasGamble: transaction["hasGamble"]?.GetValue<int>() ?? 0,
				GambleBet: -transaction["gambleBet"]?.GetValue<decimal>() ?? 0.0m,
				SystemTakeWin: transaction["systemTakeWin"]?.GetValue<int>() ?? 0,
				AdditionalData: transaction.ToJsonString());
		}
	}

	public async ValueTask<(bool IsPlayerExist, decimal Balance)> SearchPlayerInformationAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 15,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
				apiUrl,
				domain,
				encryptedData,
				"GetPlayerData",
				cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		return status is not "7501" and not "0000"
			? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
			: status == "7501"
			? (false, 0)
			: (true, jsonNode["data"]![0]!["balance"]!.GetValue<decimal>());
	}

	public async ValueTask<(bool IsPlayerExist, string GameUrl)> GetTokenAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string playerCode, string gameType, string machineType, string lang, CancellationToken cancellationToken = default)
	{
		var data = new
		{
			action = 11,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
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
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
				apiUrl,
				domain,
				encryptedData,
				"GetToken",
				cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		return status is not "7501" and not "0000"
			? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
			: status == "7501"
			? (false, string.Empty)
			: (true, jsonNode["path"]!.ToString());
	}

	public async ValueTask PlayerLogoutAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)

	{
		var data = new
		{
			action = 17,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography
				.GetAesEncryptAsync(
					jsonString,
					aesKey,
					aesIV,
					cancellationToken)
				.ConfigureAwait(false);

		var jsonNode = await PostAsync(
				apiUrl,
				domain,
				encryptedData,
				"PlayerLogout",
				cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		if (status is not "0000" and not "7405")
		{
			throw new GameProvisioningErrorException(status, errText ?? string.Empty);
		}
	}

	public async ValueTask<TransactionResp> WithdrawalAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, string serialNo, decimal amount, CancellationToken cancellationToken = default)
	{
		var retryPolicy = Policy
				.HandleResult<JsonNode>(r =>
					r["status"]!.ToString() == "6901")
					.WaitAndRetryAsync(_gameInterfaceSetting.WithdrawalRetryCount,
						retryAttempt => TimeSpan.FromSeconds(_gameInterfaceSetting.WithdrawalIntervalSeconds),
						(response, retryTime, context) => logger.LogWarning("Response:{response}", JsonSerializer.Serialize(response)));

		var jsonNode = await retryPolicy.ExecuteAsync(async () =>
		{
			var data = new
			{
				action = 19,
				ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
				parent = agent,
				uid = playerCode,
				serialNo,
				allCashOutFlag = 0,
				amount = -amount
			};
			var jsonString = JsonSerializer.Serialize(data);
			var encryptedData = await aesCryptography
					.GetAesEncryptAsync(
						jsonString,
						aesKey,
						aesIV,
						cancellationToken)
					.ConfigureAwait(false);

			return await PostAsync(
					apiUrl,
					domain,
					encryptedData,
					"Withdrawal",
					cancellationToken)
				.ConfigureAwait(false);
		}).ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();

		return status.Equals("6009", StringComparison.OrdinalIgnoreCase)
			? throw new Status_6009_Exception(amount)
			: status.Equals("6010", StringComparison.OrdinalIgnoreCase)
			? throw new Status_6010_Exception(amount)
			: !status.Equals("0000", StringComparison.OrdinalIgnoreCase)
			? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
			: new TransactionResp(
				SerialNo: jsonNode["serialNo"]!.ToString(),
				Resp: jsonNode.ToJsonString());
	}

	public async ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new
		{
			action = 52,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent,
			uid = playerCode,
		};
		var jsonString = JsonSerializer.Serialize(request);
		var encryptedData = await aesCryptography.GetAesEncryptAsync(
			jsonString,
			aesKey,
			aesIV,
			cancellationToken)
		.ConfigureAwait(false);

		var jsonNode = await PostAsync(
			apiUrl,
			domain,
			encryptedData,
			"PlayerOnline",
			cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();

		if (status is not "0000" and not "7505")
		{
			var errText = jsonNode["err_text"]?.ToString() ?? string.Empty;
			throw new GameProvisioningErrorException(status, errText);
		}

		return status is "0000";
	}

	public async ValueTask PlayerLogoutAllAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, CancellationToken cancellationToken = default)

	{
		var data = new
		{
			action = 58,
			ts = timeProvider.GetUtcNow().ToUnixTimeMilliseconds(),
			parent = agent
		};
		var jsonString = JsonSerializer.Serialize(data);
		var encryptedData = await aesCryptography.GetAesEncryptAsync(
			jsonString,
			aesKey,
			aesIV,
			cancellationToken)
			.ConfigureAwait(false);

		var jsonNode = await PostAsync(
			apiUrl,
			domain,
			encryptedData,
			"PlayerLogoutAll",
			cancellationToken)
			.ConfigureAwait(false);

		var status = jsonNode["status"]!.ToString();
		var errText = jsonNode["err_text"]?.ToString();
		if (status is not "0000")
		{
			throw new GameProvisioningErrorException(status, errText ?? string.Empty);
		}
	}

	private async ValueTask<JsonNode> PostAsync(Uri apiUrl, string domain, string encryptedData, string action, CancellationToken cancellationToken = default)
	{
		var logPrefix = $"{nameof(GameApiClient)}.{action}".ToLower();

		if (string.IsNullOrWhiteSpace(encryptedData))
		{
			throw new EncryptedDataEmptyException();
		}

		var parameters = new Dictionary<string, string>()
	{
		{"dc",domain},
		{"x",encryptedData}
	};

		var url = new Uri(apiUrl, "/apiRequest.do");

		var sw = Stopwatch.StartNew();

		var content = new FormUrlEncodedContent(parameters);
		var httpResponseMessage = await httpClient
				.PostAsync(
					url,
					content,
					cancellationToken)
				.ConfigureAwait(false);

		sw.Stop();

		var requestMessage = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		var responseMessage = default(string);
		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr
				.ReadToEndAsync(
				cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		var apiLogEvent = new GameApiLogEvent
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = requestMessage.NoLongerThan(4096),
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};

		logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return JsonNode.Parse(responseMessage)!;
	}
}
