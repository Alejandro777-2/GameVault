using System.ComponentModel.DataAnnotations;
using GameVault.Models;

namespace GameVault.ViewModels;

public class TradeOfferFormViewModel
{
    public int AssetId { get; set; }

    // Asset preview (populated by controller, not submitted)
    public string AssetTitle { get; set; } = string.Empty;
    public string? AssetImageUrl { get; set; }
    public Platform AssetPlatform { get; set; }
    public int AssetYear { get; set; }
    public Condition AssetCondition { get; set; }

    [Required(ErrorMessage = "El tipo de oferta es obligatorio.")]
    [Display(Name = "Tipo de oferta")]
    public TradeType Type { get; set; }

    [Range(0, 99999, ErrorMessage = "El precio debe estar entre 0 y 99,999.")]
    [Display(Name = "Precio (USD)")]
    public decimal? Price { get; set; }

    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    [Display(Name = "¿Qué buscas a cambio?")]
    public string? Description { get; set; }
}
