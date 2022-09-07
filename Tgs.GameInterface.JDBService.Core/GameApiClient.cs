using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.GameInterface.JDBService.Core.Exceptions;
using Tgs.GameInterface.JDBService.Core.Models;

namespace Tgs.GameInterface.JDBService.Core;

internal class GameApiClient : IGameApiClient
{
    private readonly GameInterfaceSettingOptions _gameInterfaceSetting;
    private readonly ILogger<GameApiClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly ISystemClock _systemClock;
    private readonly IAesCryptography _aesCryptography;

    public GameApiClient(
        IOptions<GameInterfaceSettingOptions> options,
        ILogger<GameApiClient> logger,
        HttpClient httpClient,
        ISystemClock systemClock,
        IAesCryptography aesCryptography)
    {
        _gameInterfaceSetting = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _systemClock = systemClock ?? throw new ArgumentNullException(nameof(systemClock));
        _aesCryptography = aesCryptography ?? throw new ArgumentNullException(nameof(aesCryptography));
    }

    public async ValueTask<bool> CheckCashTransferAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        string serialNo,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            action = 55,
            ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
            parent = agent,
            serialNo
        };
        var jsonString = JsonSerializer.Serialize(data);
        var encryptedData = await _aesCryptography
                .GetAesEncryptAsync(
                    jsonString,
                    partnerKey,
                    partnerIV,
                    cancellationToken)
                .ConfigureAwait(false);

        var jsonNode = await PostAsync(
                    apiUrl,
                    domain,
                    encryptedData,
                    "CheckTransactions",
                    cancellationToken)
                .ConfigureAwait(false);

        var status = jsonNode["status"]!.ToString();
        var errText = jsonNode["err_text"]?.ToString();

        return !status.Equals("0000", StringComparison.OrdinalIgnoreCase)
            ? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
            : true;
    }

    public async ValueTask CreatePlayerAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            action = 12,
            ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            name = playerCode,
            credit_allocated = 0
        };
        var jsonString = JsonSerializer.Serialize(data);
        var encryptedData = await _aesCryptography
                .GetAesEncryptAsync(
                    jsonString,
                    partnerKey,
                    partnerIV,
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

    public async ValueTask<TransactionResult> DepositAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        string playerCode,
        string serialNo,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            action = 19,
            ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode,
            serialNo,
            allCashOutFlag = 0,
            amount
        };
        var jsonString = JsonSerializer.Serialize(data);
        var encryptedData = await _aesCryptography
                .GetAesEncryptAsync(
                    jsonString,
                    partnerKey,
                    partnerIV,
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
        var cuiabaTime = TimeZoneInfo.FindSystemTimeZoneById("Central Brazilian Standard Time");//(UTC-04:00) Cuiaba

        return status.Equals("6009", StringComparison.OrdinalIgnoreCase)
            ? throw new Status_6009_Exception(amount)
            : status.Equals("6010", StringComparison.OrdinalIgnoreCase)
            ? throw new Status_6010_Exception(amount)
            : !status.Equals("0000", StringComparison.OrdinalIgnoreCase)
            ? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
            : new TransactionResult(
            Pid: long.Parse(jsonNode["pid"]!.ToString()),
            SerialNo: jsonNode["serialNo"]!.ToString(),
            PayDate: new DateTimeOffset(DateTime.Parse(jsonNode["payDate"]!.ToString()), cuiabaTime.GetUtcOffset(DateTime.Parse(jsonNode["payDate"]!.ToString()))),
            Amount: decimal.Parse(jsonNode["amount"]!.ToString()),
            UserBalance: decimal.Parse(jsonNode["userBalance"]!.ToString()),
            AgentCashBalance: decimal.Parse(jsonNode["agentCashBalance"]!.ToString()));
    }

    public ValueTask GetHistoryAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        DateTimeOffset starttime,
        DateTimeOffset endtime,
        CancellationToken cancellationToken = default)
        => throw new NotImplementedException();

    public async ValueTask<(bool IsPlayerExist, decimal Balance)> SearchPlayerInformationAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        string playerCode,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            action = 15,
            ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode
        };
        var jsonString = JsonSerializer.Serialize(data);
        var encryptedData = await _aesCryptography
                .GetAesEncryptAsync(
                    jsonString,
                    partnerKey,
                    partnerIV,
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

    public async ValueTask<(bool IsPlayerExist, string GameUrl)> GetTokenAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string playerCode,
        string gameType,
        string machineType,
        string lang,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            action = 11,
            ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
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
        var encryptedData = await _aesCryptography
                .GetAesEncryptAsync(
                    jsonString,
                    partnerKey,
                    partnerIV,
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

    public async ValueTask KickOutPlayerAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        string playerCode,
        CancellationToken cancellationToken = default)

    {
        var data = new
        {
            action = 17,
            ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
            parent = agent,
            uid = playerCode
        };
        var jsonString = JsonSerializer.Serialize(data);
        var encryptedData = await _aesCryptography
                .GetAesEncryptAsync(
                    jsonString,
                    partnerKey,
                    partnerIV,
                    cancellationToken)
                .ConfigureAwait(false);

        var jsonNode = await PostAsync(
                apiUrl,
                domain,
                encryptedData,
                "KickPlayer",
                cancellationToken)
            .ConfigureAwait(false);

        var status = jsonNode["status"]!.ToString();
        var errText = jsonNode["err_text"]?.ToString();
        if (status is not "0000" and not "7405")
        {
            throw new GameProvisioningErrorException(status, errText ?? string.Empty);
        }
    }

    public async ValueTask<TransactionResult> WithdrawalAsync(
        Uri apiUrl,
        string domain,
        string partnerKey,
        string partnerIV,
        string agent,
        string playerCode,
        string serialNo,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var retryPolicy = Policy
                .HandleResult<JsonNode>(r =>
                    r["status"]!.ToString() == "6901")
                    .WaitAndRetryAsync(_gameInterfaceSetting.WithdrawalRetryCount,
                        retryAttempt => TimeSpan.FromSeconds(_gameInterfaceSetting.WithdrawalIntervalSeconds),
                        (response, retryTime, context) => _logger.LogWarning("Response:{response}", JsonSerializer.Serialize(response)));

        var jsonNode = await retryPolicy.ExecuteAsync(async () =>
        {
            var data = new
            {
                action = 19,
                ts = _systemClock.UtcNow.ToUnixTimeMilliseconds(),
                parent = agent,
                uid = playerCode,
                serialNo,
                allCashOutFlag = 0,
                amount = -amount
            };
            var jsonString = JsonSerializer.Serialize(data);
            var encryptedData = await _aesCryptography
                    .GetAesEncryptAsync(
                        jsonString,
                        partnerKey,
                        partnerIV,
                        cancellationToken)
                    .ConfigureAwait(false);

            return await PostAsync(
                    apiUrl,
                    domain,
                    encryptedData,
                    "Withdrawal",
                    cancellationToken)
                .ConfigureAwait(false);
        });

        var status = jsonNode["status"]!.ToString();
        var errText = jsonNode["err_text"]?.ToString();
        var cuiabaTime = TimeZoneInfo.FindSystemTimeZoneById("Central Brazilian Standard Time");//(UTC-04:00) Cuiaba

        return status.Equals("6009", StringComparison.OrdinalIgnoreCase)
            ? throw new Status_6009_Exception(amount)
            : status.Equals("6010", StringComparison.OrdinalIgnoreCase)
            ? throw new Status_6010_Exception(amount)
            : !status.Equals("0000", StringComparison.OrdinalIgnoreCase)
            ? throw new GameProvisioningErrorException(status, errText ?? string.Empty)
            : new TransactionResult(
            Pid: long.Parse(jsonNode["pid"]!.ToString()),
            SerialNo: jsonNode["serialNo"]!.ToString(),
            PayDate: new DateTimeOffset(DateTime.Parse(jsonNode["payDate"]!.ToString()), cuiabaTime.GetUtcOffset(DateTime.Parse(jsonNode["payDate"]!.ToString()))),
            Amount: decimal.Parse(jsonNode["amount"]!.ToString()),
            UserBalance: decimal.Parse(jsonNode["userBalance"]!.ToString()),
            AgentCashBalance: decimal.Parse(jsonNode["agentCashBalance"]!.ToString()));
    }

    private async ValueTask<JsonNode> PostAsync(
        Uri apiUrl,
        string domain,
        string encryptedData,
        string action,
        CancellationToken cancellationToken = default)
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
        var httpResponseMessage = await _httpClient
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
            responseMessage = await sr.ReadToEndAsync() ?? string.Empty;
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

        _logger.LogInformation("{JsonSerializer.Serialize(apiLogEvent)}", JsonSerializer.Serialize(apiLogEvent));

        _ = httpResponseMessage.EnsureSuccessStatusCode();

        return JsonNode.Parse(responseMessage)!;
    }
}
