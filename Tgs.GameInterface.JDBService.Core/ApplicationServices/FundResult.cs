namespace Tgs.GameInterface.JDBService.Core.ApplicationServices;

public record FundResult(
    long Pid,
    string SerialNo,
    DateTimeOffset PayDate,
    decimal Amount,
    decimal UserBalance,
    decimal AgentCashBalance);
