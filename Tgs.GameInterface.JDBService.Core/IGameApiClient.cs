using Tgs.GameInterface.JDBService.Core.Models;

namespace Tgs.GameInterface.JDBService.Core;

public interface IGameApiClient
{
	ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default);

	ValueTask CreatePlayerAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<(bool IsPlayerExist, decimal Balance)> SearchPlayerInformationAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<TransactionResp> DepositAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, string serialNo, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<bool> CheckCashTransferAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string serialNo, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameDetailTransactionResp> SearchGameTransactionDetailsAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameDetailTransactionResp> SearchGameOldTransactionDetailsAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

	ValueTask<(bool IsPlayerExist, string GameUrl)> GetTokenAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string playerCode, string gType, string mType, string lang, CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<TransactionResp> WithdrawalAsync(Uri apiUrl, string domain, string aesKey, string aesIV, string agent, string playerCode, string serialNo, decimal amount, CancellationToken cancellationToken = default);
}
