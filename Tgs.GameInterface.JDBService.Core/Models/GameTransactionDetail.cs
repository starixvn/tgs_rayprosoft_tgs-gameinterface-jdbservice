using System.Text.Json.Serialization;

namespace Tgs.GameInterface.JDBService.Core.Models;

public record GameTransactionDetail
{
    [JsonPropertyName("seqNo")]
    public long SeqNo { get; set; }
    [JsonPropertyName("playerId")]
    public string PlayerId { get; set; } = default!;
    [JsonPropertyName("gType")]
    public int GameType { get; set; }
    [JsonPropertyName("mtype")]
    public int MachineType { get; set; }
    [JsonPropertyName("gameDate")]
    public DateTimeOffset GameDate { get; set; }
    [JsonPropertyName("bet")]
    public decimal Bet { get; set; }
    [JsonPropertyName("win")]
    public decimal Win { get; set; }
    [JsonPropertyName("total")]
    public decimal Total { get; set; }
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = default!;
    [JsonPropertyName("jackpot")]
    public decimal Jackpot { get; set; }
    [JsonPropertyName("jackpotContribute")]
    public decimal JackpotContribute { get; set; }
    [JsonPropertyName("denom")]
    public decimal Denom { get; set; }
    [JsonPropertyName("lastModifyTime")]
    public DateTimeOffset LastModifyTime { get; set; }
    [JsonPropertyName("playerIp")]
    public string PlayerIp { get; set; } = default!;
    [JsonPropertyName("clientType")]
    public string ClientType { get; set; } = default!;
    [JsonPropertyName("hasFreegame")]
    public int HasFreegame { get; set; }
    [JsonPropertyName("hasGamble")]
    public int HasGamble { get; set; }
    [JsonPropertyName("gambleBet")]
    public decimal GambleBet { get; set; }
    [JsonPropertyName("systemTakeWin")]
    public int SystemTakeWin { get; set; }
}
