using Tgs.GameInterface.JDBService.Protos;

namespace Tgs.GameInterface.JDBService.Client
{
    public interface IGameInterfaceJDBClient
    {
        ValueTask<FundReply> FundInAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            string serialNo,
            decimal amount,
            CancellationToken cancellationToken = default);

        ValueTask<FundReply> FundOutAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            string serialNo,
            decimal amount,
            CancellationToken cancellationToken = default);

        ValueTask<bool> FundStatusAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string serialNo,
            CancellationToken cancellationToken = default);

        ValueTask<GameUrlReply> GameUrlAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string playerCode,
            string gType,
            string mType,
            string lang,
            CancellationToken cancellationToken = default);

        ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            CancellationToken cancellationToken = default);

        ValueTask PlayerCreateAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            CancellationToken cancellationToken = default);

        ValueTask PlayerKickAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            CancellationToken cancellationToken = default);
    }
}
