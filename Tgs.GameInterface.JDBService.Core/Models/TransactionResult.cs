namespace Tgs.GameInterface.JDBService.Core.Models;

public record TransactionResult(
    long Pid,
    string SerialNo,
    DateTimeOffset PayDate,
    decimal Amount,
    decimal UserBalance,
    decimal AgentCashBalance);
