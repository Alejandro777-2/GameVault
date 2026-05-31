using System.ComponentModel.DataAnnotations;
using GameVault.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameVault.ViewModels;

public class AssetCreateViewModel
{
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

    [StringLength(2000, ErrorMessage = "La descripción no puede superar los 2000 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El valor estimado es obligatorio.")]
    [Range(0, 99999, ErrorMessage = "El valor estimado debe estar entre 0 y 99,999.")]
    [Display(Name = "Valor estimado (USD)")]
    public decimal EstimatedValue { get; set; }

    [Display(Name = "Imagen del activo")]
    public IFormFile? ImageFile { get; set; }

    public IEnumerable<SelectListItem> Platforms { get; set; } = [];
    public IEnumerable<SelectListItem> Regions { get; set; } = [];
    public IEnumerable<SelectListItem> Conditions { get; set; } = [];
}
