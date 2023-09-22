namespace Tgs.GameInterface.JDBService.Core.Models;

public record GameDetailTransactionResp(
    string HistoryId,
    string PlayerId,
    int GameType,
    int MachineType,
    DateTimeOffset GameDate,
    decimal Bet,
    decimal Win,
    decimal Total,
    string Currency,
    decimal Jackpot,
    decimal JackpotContribute,
    decimal Denom,
    DateTimeOffset LastModifyTime,
    string PlayerIp,
    string ClientType,
    int HasFreegame,
    int HasGamble,
    decimal GambleBet,
    int SystemTakeWin,
    string AdditionalData);
