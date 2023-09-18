using Tgs.GameInterface.JDBService.Protos;

namespace Tgs.GameInterface.JDBService.Client;

public interface IGameInterfaceJDBClient
{
    ValueTask<string> CheckingServiceAsync(
        string machineName,
        CancellationToken cancellationToken = default);

    ValueTask<FundReply> FundInAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        string playerCode,
        string serialNo,
        decimal amount,
        CancellationToken cancellationToken = default);

    ValueTask<FundReply> FundOutAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        string playerCode,
        string serialNo,
        decimal amount,
        CancellationToken cancellationToken = default);

    ValueTask<bool> FundStatusAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        string serialNo,
        CancellationToken cancellationToken = default);

    ValueTask<GameUrlReply> GameUrlAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string playerCode,
        string gameType,
        string machineType,
        string lang,
        CancellationToken cancellationToken = default);

    [Obsolete("已過時，請改使用GameWager")]
    IAsyncEnumerable<GameHistoryInfo> GameHistoryAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    [Obsolete("已過時，請改使用GameWagerOld")]
    IAsyncEnumerable<GameHistoryInfo> GameOldHistoryAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameWagerInfo> GameWagerAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameWagerInfo> GameWagerOldAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        string playerCode,
        CancellationToken cancellationToken = default);

    ValueTask PlayerCreateAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        string playerCode,
        CancellationToken cancellationToken = default);

    ValueTask PlayerKickAsync(
        Uri apiUrl,
        string domain,
        string aesKey,
        string aesIV,
        string agent,
        string playerCode,
        CancellationToken cancellationToken = default);
}
