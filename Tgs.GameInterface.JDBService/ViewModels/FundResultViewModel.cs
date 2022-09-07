namespace Tgs.GameInterface.JDBService.ViewModels;

public record FundResultViewModel(
    long Pid,
    string SerialNo,
    DateTimeOffset PayDate,
    decimal Amount,
    decimal UserBalance,
    decimal AgentCashBalance);
