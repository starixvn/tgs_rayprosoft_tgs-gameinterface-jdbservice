using System.Text.Json.Serialization;

namespace Tgs.GameInterface.JDBService.Core.Models;

public record TransactionResponse
{
    [JsonPropertyName("pid")]
    public long Pid { get; set; }
    [JsonPropertyName("serialNo")]
    public string SerialNo { get; set; } = default!;
    [JsonPropertyName("payDate")]
    public DateTimeOffset PayDate { get; set; }
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
    [JsonPropertyName("userBalance")]
    public decimal UserBalance { get; set; }
    [JsonPropertyName("agentCashBalance")]
    public decimal AgentCashBalance { get; set; }
}
