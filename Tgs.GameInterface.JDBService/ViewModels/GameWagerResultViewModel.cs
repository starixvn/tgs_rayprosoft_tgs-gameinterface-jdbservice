using Tgs.GameInterface.JDBService.Core.ApplicationServices;
namespace Tgs.GameInterface.JDBService.ViewModels;

public record GameWagerResultViewModel(
    string PlayerCode,
    string ExternalRoundId,
    string ExternalWagerId,
    string ExternalGameId,
    bool FreeGameState,
    bool GambleState,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal Bet,
    decimal Settle,
    decimal WinLoss,
    string AdditionalData);
