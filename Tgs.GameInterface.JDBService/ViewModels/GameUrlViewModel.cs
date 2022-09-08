using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.JDBService.ViewModels;

public record GameUrlViewModel
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
    public string PlayerCode { get; init; } = default!;
    [Required]
    public string GameType { get; init; } = default!;
    [Required]
    public string MachineType { get; init; } = default!;

    public string Lang { get; init; } = default!;
}
