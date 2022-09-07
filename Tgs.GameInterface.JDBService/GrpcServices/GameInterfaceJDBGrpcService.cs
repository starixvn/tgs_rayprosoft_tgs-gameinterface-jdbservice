using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;
using Tgs.GameInterface.JDBService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.GrpcServices;

public class GameInterfaceJDBGrpcService : GameInterfaceJDBService.GameInterfaceJDBServiceBase
{
    private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
    private readonly ICommandService<PlayerCreateCommand> _playerCreateService;
    private readonly ICommandService<PlayerKickCommand> _playerKickService;
    private readonly IGetService<FundInQuery, FundResult> _fundInService;
    private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
    private readonly IGetService<FundStatusQuery, bool> _fundStatusService;
    private readonly IGetService<GameUrlQuery, GameUrlResult> _gameUrlService;

    public GameInterfaceJDBGrpcService(
        IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
        ICommandService<PlayerCreateCommand> PlayerCreateService,
        ICommandService<PlayerKickCommand> playerKickService,
        IGetService<FundInQuery, FundResult> fundInService,
        IGetService<FundOutQuery, FundResult> fundOutService,
        IGetService<FundStatusQuery, bool> fundStatusService,
        IGetService<GameUrlQuery, GameUrlResult> gameUrlService
        )
    {
        _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
        _playerCreateService = PlayerCreateService ?? throw new ArgumentNullException(nameof(PlayerCreateService));
        _playerKickService = playerKickService ?? throw new ArgumentNullException(nameof(playerKickService));
        _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        _fundStatusService = fundStatusService ?? throw new ArgumentNullException(nameof(fundStatusService));
        _gameUrlService = gameUrlService ?? throw new ArgumentNullException(nameof(gameUrlService));
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.Agent,
        request.PlayerCode,
        request.SerialNo,
        request.Amount);

        var result = await _fundInService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply()
        {
            Pid = result.Pid,
            SerialNo = result.SerialNo,
            PayDate = result.PayDate.ToTimestamp(),
            Amount = result.Amount,
            UserBalance = result.UserBalance,
            AgentCashBalance = result.AgentCashBalance
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.Agent,
        request.PlayerCode,
        request.SerialNo,
        request.Amount);

        var result = await _fundOutService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundReply()
        {
            Pid = result.Pid,
            SerialNo = result.SerialNo,
            PayDate = result.PayDate.ToTimestamp(),
            Amount = result.Amount,
            UserBalance = result.UserBalance,
            AgentCashBalance = result.AgentCashBalance
        };
    }

    public override async Task<FundStatusReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.Agent,
        request.SerialNo);

        var result = await _fundStatusService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new FundStatusReply()
        {
            IsOrderExist = result
        };
    }

    public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
    {
        var query = new GameUrlQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.PlayerCode,
        request.GameType,
        request.MachineType,
        request.Lang);

        var result = await _gameUrlService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new GameUrlReply()
        {
            IsPlayerExist = result.IsPlayerExist,
            GameUrl = result.GameUrl
        };
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.Agent,
        request.PlayerCode);

        var result = await _playerBalanceService
            .GetAsync(
                query: query,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new PlayerBalanceReply()
        {
            IsPlayerExist = result.IsPlayerExist,
            Balance = result.Balance
        };
    }

    public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
    {
        var command = new PlayerCreateCommand(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.Agent,
        request.PlayerCode);

        await _playerCreateService
            .ExecuteAsync(
                command: command,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply()
        {
            Status = true
        };
    }

    public override async Task<BaseReply> PlayerKick(PlayerKickRequest request, ServerCallContext context)
    {
        var command = new PlayerKickCommand(
        new Uri(request.ApiUrl),
        request.Domain,
        request.PartnerKey,
        request.PartnerIv,
        request.Agent,
        request.PlayerCode);

        await _playerKickService
            .ExecuteAsync(
                command: command,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply()
        {
            Status = true
        };
    }
}
