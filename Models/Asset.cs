using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameVault.Models;

public class Asset
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El título debe tener entre 2 y 200 caracteres.")]
    [Display(Name = "Título")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La plataforma es obligatoria.")]
    [Display(Name = "Plataforma")]
    public Platform Platform { get; set; }

    [Required(ErrorMessage = "El año es obligatorio.")]
    [Range(1970, 2010, ErrorMessage = "El año debe estar entre 1970 y 2010.")]
    [Display(Name = "Año de lanzamiento")]
    public int Year { get; set; }

    [Required(ErrorMessage = "La región es obligatoria.")]
    [Display(Name = "Región")]
    public Region Region { get; set; }

    [Required(ErrorMessage = "El estado de conservación es obligatorio.")]
    [Display(Name = "Estado de conservación")]
    public Condition Condition { get; set; }

    [Display(Name = "Imagen")]
    public string? ImageUrl { get; set; }

    [StringLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El valor estimado es obligatorio.")]
    [Range(0, 99999, ErrorMessage = "El valor estimado debe estar entre 0 y 99,999.")]
    [Display(Name = "Valor estimado (USD)")]
    [Column(TypeName = "TEXT")]
    public decimal EstimatedValue { get; set; }

    [Display(Name = "Tipo de oferta")]
    public TradeType OfferType { get; set; } = TradeType.Sale;

    [StringLength(300, ErrorMessage = "El texto de intercambio no puede superar los 300 caracteres.")]
    [Display(Name = "¿Qué deseas a cambio?")]
    public string? TradeWants { get; set; }

    [Required]
    public string OwnerId { get; set; } = string.Empty;

    [Display(Name = "Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "Activo")]
    public bool IsActive { get; set; } = true;

    public ApplicationUser Owner { get; set; } = null!;
    public ICollection<TradeOffer> TradeOffers { get; set; } = [];
}
