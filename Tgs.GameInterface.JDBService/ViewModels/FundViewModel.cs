using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.JDBService.ViewModels;

public record FundViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string Domain { get; init; } = default!;
    [Required]
    public string AesKey { get; init; } = default!;
    [Required]
    public string AesIV { get; init; } = default!;
    [Required]
    public string Agent { get; init; } = default!;
    [Required]
    public string PlayerCode { get; init; } = default!;
    [Required]
    public string SerialNo { get; init; } = default!;
    [Required]
    [Range(minimum: 0, maximum: int.MaxValue)]
    public decimal Amount { get; init; }
}
