using System.ComponentModel.DataAnnotations;

namespace PlayerAuctionApp.Models;

public sealed class NewTeamInput
{
    [Required]
    [StringLength(60)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Select a city for the team.")]
    [StringLength(60)]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "Upload a logo for the team.")]
    public string? LogoDataUrl { get; set; }

    [Required]
    [StringLength(80)]
    public string OwnerName { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string OwnerPhone { get; set; } = string.Empty;

    [EmailAddress]
    public string? OwnerEmail { get; set; }
}
