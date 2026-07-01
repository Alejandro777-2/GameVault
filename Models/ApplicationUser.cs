using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace GameVault.Models;

public class ApplicationUser : IdentityUser
{
    [Required(ErrorMessage = "El nombre para mostrar es obligatorio.")]
    [Display(Name = "Nombre para mostrar")]
    [StringLength(80, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 80 caracteres.")]
    public string DisplayName { get; set; } = string.Empty;

    [Display(Name = "Ciudad")]
    [StringLength(100, ErrorMessage = "La ciudad no puede superar los 100 caracteres.")]
    public string? City { get; set; }

    [Display(Name = "País")]
    [StringLength(100, ErrorMessage = "El país no puede superar los 100 caracteres.")]
    public string? Country { get; set; }

    [Display(Name = "Foto de perfil")]
    public string? AvatarUrl { get; set; }

    [Display(Name = "Reputación")]
    public int ReputationScore { get; set; }

    [Display(Name = "Fecha de registro")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Latitud")]
    public double? Latitude { get; set; }

    [Display(Name = "Longitud")]
    public double? Longitude { get; set; }

    public ICollection<Asset> Assets { get; set; } = [];
    public ICollection<TradeOffer> TradeOffers { get; set; } = [];
    public ICollection<Review> ReviewsGiven { get; set; } = [];
    public ICollection<Review> ReviewsReceived { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
}
