using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;
using Tgs.GameInterface.JDBService.Core.Models;
using Tgs.GameInterface.JDBService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.GrpcServices;

public class GameInterfaceJDBGrpcService : GameInterfaceJDBService.GameInterfaceJDBServiceBase
{
    private readonly ILogger<GameInterfaceJDBGrpcService> _logger;
    private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
    private readonly ICommandService<PlayerCreateCommand> _playerCreateService;
    private readonly ICommandService<PlayerKickCommand> _playerKickService;
    private readonly IGetService<FundInQuery, FundResult> _fundInService;
    private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
    private readonly IGetService<FundStatusQuery, bool> _fundStatusService;
    private readonly IGetService<GameUrlQuery, GameUrlResult> _gameUrlService;
    private readonly IQueryService<GameHistoryQuery, GameHistoryResult> _gameHistoryService;
    private readonly IQueryService<GameOldHistoryQuery, GameHistoryResult> _gameOldHistoryService;
    private readonly IQueryService<GameWagerQuery, GameWagerResult> _gameWagerService;
    private readonly IQueryService<GameWagerOldQuery, GameWagerResult> _gameWagerOldService;

    public GameInterfaceJDBGrpcService(
        ILogger<GameInterfaceJDBGrpcService> logger,
        IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
        ICommandService<PlayerCreateCommand> PlayerCreateService,
        ICommandService<PlayerKickCommand> playerKickService,
        IGetService<FundInQuery, FundResult> fundInService,
        IGetService<FundOutQuery, FundResult> fundOutService,
        IGetService<FundStatusQuery, bool> fundStatusService,
        IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
        IQueryService<GameHistoryQuery, GameHistoryResult> gameHistoryService,
        IQueryService<GameOldHistoryQuery, GameHistoryResult> gameOldHistoryService,
        IQueryService<GameWagerQuery, GameWagerResult> gameWagerService,
        IQueryService<GameWagerOldQuery, GameWagerResult> gameWagerOldService
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
        _playerCreateService = PlayerCreateService ?? throw new ArgumentNullException(nameof(PlayerCreateService));
        _playerKickService = playerKickService ?? throw new ArgumentNullException(nameof(playerKickService));
        _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        _fundStatusService = fundStatusService ?? throw new ArgumentNullException(nameof(fundStatusService));
        _gameUrlService = gameUrlService ?? throw new ArgumentNullException(nameof(gameUrlService));
        _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        _gameOldHistoryService = gameOldHistoryService ?? throw new ArgumentNullException(nameof(gameOldHistoryService));
        _gameWagerService = gameWagerService ?? throw new ArgumentNullException(nameof(gameWagerService));
        _gameWagerOldService = gameWagerOldService ?? throw new ArgumentNullException(nameof(gameWagerOldService));
    }

    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        _logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

        return Task.FromResult(new CheckingServiceReply
        {
            MachineName = Environment.MachineName
        });
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
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
            SerialNo = result.SerialNo,
            Resp = result.Resp
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
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
            SerialNo = result.SerialNo,
            Resp = result.Resp
        };
    }

    public override async Task<FundStatusReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
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

    public override async Task<GameHistoryReply> GameHistory(GameHistoryRequest request, ServerCallContext context)
    {
        var query = new GameHistoryQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
        request.Agent,
        request.StartTime.ToDateTime(),
        request.EndTime.ToDateTime());

        var results = await _gameHistoryService
            .QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync()
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistoryResult>();

        var reply = new GameHistoryReply();

        if (results.Any())
        {
            foreach (var result in results)
            {
                reply.Items.Add(new GameHistoryInfo()
                {
                    PlayerCode = result.PlayerCode,
                    ExternalRoundId = result.ExternalRoundId,
                    ExternalGameId = result.ExternalGameId,
                    ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
                    Bet = result.Bet,
                    ValidBet = result.ValidBet,
                    PlayerWinLoss = result.PlayerWinLoss,
                    Turnover = result.Turnover,
                    Settle = result.Settle,
                    AdditionalData = result.AdditionalData
                });
            }
        }

        return reply;
    }

    public override async Task<GameHistoryReply> GameOldHistory(GameHistoryRequest request, ServerCallContext context)
    {
        var query = new GameOldHistoryQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
        request.Agent,
        request.StartTime.ToDateTime(),
        request.EndTime.ToDateTime());

        var results = await _gameOldHistoryService
            .QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync()
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistoryResult>();

        var reply = new GameHistoryReply();

        if (results.Any())
        {
            foreach (var result in results)
            {
                reply.Items.Add(new GameHistoryInfo()
                {
                    PlayerCode = result.PlayerCode,
                    ExternalRoundId = result.ExternalRoundId,
                    ExternalGameId = result.ExternalGameId,
                    ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
                    Bet = result.Bet,
                    ValidBet = result.ValidBet,
                    PlayerWinLoss = result.PlayerWinLoss,
                    Turnover = result.Turnover,
                    Settle = result.Settle,
                    AdditionalData = result.AdditionalData
                });
            }
        }

        return reply;
    }

    public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
    {
        var query = new GameUrlQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
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

    public override async Task<GameWagerReply> GameWager(GameWagerRequest request, ServerCallContext context)
    {
        var query = new GameWagerQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
        request.Agent,
        request.StartTime.ToDateTime(),
        request.EndTime.ToDateTime());

        var results = await _gameWagerService
            .QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync()
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        var reply = new GameWagerReply();

        if (results.Any())
        {
            foreach (var result in results)
            {
                reply.Items.Add(new GameWagerInfo()
                {
                    PlayerCode = result.PlayerCode,
                    ExternalWagerId = result.ExternalWagerId,
                    ExternalGameId = result.ExternalGameId,
                    FreeGameState = result.FreeGameState,
                    GambleState = result.GambleState,
                    ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
                    Bet = result.Bet,
                    Settle = result.Settle,
                    WinLoss = result.WinLoss,
                    AdditionalData = result.AdditionalData
                });
            }
        }

        return reply;
    }

    public override async Task<GameWagerReply> GameWagerOld(GameWagerRequest request, ServerCallContext context)
    {
        var query = new GameWagerOldQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
        request.Agent,
        request.StartTime.ToDateTime(),
        request.EndTime.ToDateTime());

        var results = await _gameWagerOldService
            .QueryAsync(
            query: query,
            cancellationToken: context.CancellationToken)
            .ToArrayAsync()
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameWagerResult>();

        var reply = new GameWagerReply();

        if (results.Any())
        {
            foreach (var result in results)
            {
                reply.Items.Add(new GameWagerInfo()
                {
                    PlayerCode = result.PlayerCode,
                    ExternalWagerId = result.ExternalWagerId,
                    ExternalGameId = result.ExternalGameId,
                    FreeGameState = result.FreeGameState,
                    GambleState = result.GambleState,
                    ExternalCreatedOn = result.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = result.ExternalClosedOn.ToTimestamp(),
                    Bet = result.Bet,
                    Settle = result.Settle,
                    WinLoss = result.WinLoss,
                    AdditionalData = result.AdditionalData
                });
            }
        }

        return reply;
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
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
        request.AesKey,
        request.AesIV,
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
        request.AesKey,
        request.AesIV,
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
