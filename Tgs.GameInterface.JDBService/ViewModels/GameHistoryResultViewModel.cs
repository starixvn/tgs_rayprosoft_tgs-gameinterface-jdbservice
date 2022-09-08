namespace Tgs.GameInterface.JDBService.ViewModels;

public record GameHistoryResultViewModel(
    string PlayerCode,
    string ExternalRoundId,
    string ExternalGameId,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal PlayerWinLoss,
    decimal Turnover,
    decimal Settle,
    string AdditionalData);
