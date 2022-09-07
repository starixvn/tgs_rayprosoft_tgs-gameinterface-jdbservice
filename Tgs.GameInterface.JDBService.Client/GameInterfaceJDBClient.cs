using Tgs.GameInterface.JDBService.Protos;

namespace Tgs.GameInterface.JDBService.Client
{
    internal class GameInterfaceJDBClient : IGameInterfaceJDBClient
    {
        private readonly GameInterfaceJDBService.GameInterfaceJDBServiceClient _client;

        public GameInterfaceJDBClient(GameInterfaceJDBService.GameInterfaceJDBServiceClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<FundReply> FundInAsync(
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
            var request = new FundRequest()
            {
                ApiUrl = apiUrl.ToString(),
                Domain = domain,
                PartnerKey = partnerKey,
                PartnerIv = partnerIV,
                Agent = agent,
                PlayerCode = playerCode,
                SerialNo = serialNo,
                Amount = amount
            };
            return await _client
                .FundInAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask<FundReply> FundOutAsync(
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
            var request = new FundRequest()
            {
                ApiUrl = apiUrl.ToString(),
                Domain = domain,
                PartnerKey = partnerKey,
                PartnerIv = partnerIV,
                Agent = agent,
                PlayerCode = playerCode,
                SerialNo = serialNo,
                Amount = amount
            };
            return await _client
                .FundOutAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask<bool> FundStatusAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string serialNo,
            CancellationToken cancellationToken = default)
        {
            var request = new FundStatusRequest()
            {
                ApiUrl = apiUrl.ToString(),
                Domain = domain,
                PartnerKey = partnerKey,
                PartnerIv = partnerIV,
                Agent = agent,
                SerialNo = serialNo
            };
            var reply = await _client
                .FundStatusAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return reply.IsOrderExist;
        }

        public ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string domain, string partnerKey, string partnerIV, string playerCode, string gType, string mType, string lang, CancellationToken cancellationToken = default) => throw new NotImplementedException();

        public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            CancellationToken cancellationToken = default)
        {
            var request = new PlayerBalanceRequest()
            {
                ApiUrl = apiUrl.ToString(),
                Domain = domain,
                PartnerKey = partnerKey,
                PartnerIv = partnerIV,
                Agent = agent,
                PlayerCode = playerCode
            };
            return await _client
                .PlayerBalanceAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask PlayerCreateAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            CancellationToken cancellationToken = default)
        {
            var request = new PlayerCreateRequest()
            {
                ApiUrl = apiUrl.ToString(),
                Domain = domain,
                PartnerKey = partnerKey,
                PartnerIv = partnerIV,
                Agent = agent,
                PlayerCode = playerCode
            };
            _ = await _client
                .PlayerCreateAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask PlayerKickAsync(
            Uri apiUrl,
            string domain,
            string partnerKey,
            string partnerIV,
            string agent,
            string playerCode,
            CancellationToken cancellationToken = default)
        {
            var request = new PlayerKickRequest()
            {
                ApiUrl = apiUrl.ToString(),
                Domain = domain,
                PartnerKey = partnerKey,
                PartnerIv = partnerIV,
                Agent = agent,
                PlayerCode = playerCode
            };
            _ = await _client
                .PlayerKickAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
