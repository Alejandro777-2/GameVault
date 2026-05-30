using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameVault.Models;

public class TradeOffer
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El artículo es obligatorio.")]
    [Display(Name = "Artículo")]
    public int AssetId { get; set; }

    [Required]
    public string OwnerId { get; set; } = string.Empty;

    [Required(ErrorMessage = "El tipo de oferta es obligatorio.")]
    [Display(Name = "Tipo de oferta")]
    public TradeType Type { get; set; }

    [Range(0, 99999, ErrorMessage = "El precio debe estar entre 0 y 99,999.")]
    [Display(Name = "Precio (USD)")]
    [Column(TypeName = "TEXT")]
    public decimal? Price { get; set; }

    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    [Display(Name = "Descripción de la oferta")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [Display(Name = "Estado")]
    public TradeStatus Status { get; set; }

    [Display(Name = "Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Fecha de cierre")]
    public DateTime? ClosedAt { get; set; }

    public Asset Asset { get; set; } = null!;
    public ApplicationUser Owner { get; set; } = null!;
}
