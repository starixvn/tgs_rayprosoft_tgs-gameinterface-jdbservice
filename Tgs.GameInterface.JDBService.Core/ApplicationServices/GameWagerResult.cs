namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record GameWagerResult(
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
