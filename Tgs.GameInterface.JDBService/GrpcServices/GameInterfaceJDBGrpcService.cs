using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.JDBService.Core.ApplicationServices;
using Tgs.GameInterface.JDBService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.JDBService.GrpcServices;

public class GameInterfaceJDBGrpcService(
    ILogger<GameInterfaceJDBGrpcService> logger,
    IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
    ICommandService<PlayerCreateCommand> PlayerCreateService,
    ICommandService<PlayerLogoutCommand> playerKickService,
    IGetService<FundInQuery, FundResult> fundInService,
    IGetService<FundOutQuery, FundResult> fundOutService,
    IGetService<FundStatusQuery, bool> fundStatusService,
    IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
    IQueryService<GameWagerQuery, GameWagerResult> gameWagerService,
    IQueryService<GameWagerOldQuery, GameWagerResult> gameWagerOldService) : GameInterfaceJDBService.GameInterfaceJDBServiceBase
{
    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

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

        var result = await fundInService
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

        var result = await fundOutService
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

        var result = await fundStatusService
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
        request.AesKey,
        request.AesIV,
        request.PlayerCode,
        request.GameType,
        request.MachineType,
        request.Lang);

        var result = await gameUrlService
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

        var results = await gameWagerService
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
                    ExternalRoundId = result.ExternalRoundId,
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

        var results = await gameWagerOldService
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
                    ExternalRoundId = result.ExternalRoundId,
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

        var result = await playerBalanceService
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

        await PlayerCreateService
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
        var command = new PlayerLogoutCommand(
        new Uri(request.ApiUrl),
        request.Domain,
        request.AesKey,
        request.AesIV,
        request.Agent,
        request.PlayerCode);

        await playerKickService
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
