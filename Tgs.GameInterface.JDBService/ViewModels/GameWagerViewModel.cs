using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.JDBService.ViewModels;

public class GameWagerViewModel
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
    public DateTime StartTime { get; init; } = default!;

    [Required]
    public DateTime EndTime { get; init; } = default!;
}
