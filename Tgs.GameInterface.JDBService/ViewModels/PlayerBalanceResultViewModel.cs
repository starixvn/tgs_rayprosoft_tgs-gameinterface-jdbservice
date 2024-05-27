namespace Tgs.GameInterface.JDBService.ViewModels;

public record PlayerBalanceResultViewModel(
	bool IsPlayerExist,
	decimal Balance);
