using System.ComponentModel.DataAnnotations;
using PlayerAuctionApp.Domain;

namespace PlayerAuctionApp.Models;

public sealed class NewPlayerInput
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Enter the player's date of birth.")]
    public DateOnly? DateOfBirth { get; set; }

    [Required(ErrorMessage = "Select the player's city.")]
    [StringLength(60)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Choose a player role.")]
    public PlayerRole? Role { get; set; }

    [Required(ErrorMessage = "Choose the player's batting hand.")]
    public BattingHand? BattingHand { get; set; }

    [StringLength(60)]
    public string? BowlingStyle { get; set; }

    [Range(1, 1_000_000, ErrorMessage = "Base price must be between 1 and 1,000,000 points.")]
    public int BasePricePoints { get; set; } = 5_000;

    public string? PhotoDataUrl { get; set; }
}
