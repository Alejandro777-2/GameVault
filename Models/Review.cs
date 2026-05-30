using System.ComponentModel.DataAnnotations;

namespace GameVault.Models;

public class Review
{
    public int Id { get; set; }

    [Required]
    public string FromUserId { get; set; } = string.Empty;

    [Required]
    public string ToUserId { get; set; } = string.Empty;

    [Required(ErrorMessage = "La calificación es obligatoria.")]
    [Range(1, 5, ErrorMessage = "La calificación debe ser entre 1 y 5.")]
    [Display(Name = "Calificación")]
    public int Rating { get; set; }

    [StringLength(500, ErrorMessage = "El comentario no puede superar los 500 caracteres.")]
    [Display(Name = "Comentario")]
    public string? Comment { get; set; }

    [Display(Name = "Fecha")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Display(Name = "De")]
    public ApplicationUser FromUser { get; set; } = null!;

    [Display(Name = "Para")]
    public ApplicationUser ToUser { get; set; } = null!;
}
