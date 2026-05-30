using System.ComponentModel.DataAnnotations;

namespace GameVault.Models;

public enum Platform
{
    [Display(Name = "NES")] NES,
    [Display(Name = "Super Nintendo")] SNES,
    [Display(Name = "Nintendo 64")] N64,
    [Display(Name = "GameCube")] GameCube,
    [Display(Name = "Wii")] Wii,
    [Display(Name = "PlayStation")] PS1,
    [Display(Name = "PlayStation 2")] PS2,
    [Display(Name = "PlayStation 3")] PS3,
    [Display(Name = "Dreamcast")] Dreamcast,
    [Display(Name = "Game Boy")] GameBoy,
    [Display(Name = "Game Boy Advance")] GBA,
    [Display(Name = "Nintendo DS")] DS,
    [Display(Name = "Otra")] Other
}

public enum Region
{
    [Display(Name = "NTSC (América)")] NTSC_US,
    [Display(Name = "PAL (Europa)")] PAL,
    [Display(Name = "NTSC (Japón)")] NTSC_JP
}

public enum Condition
{
    [Display(Name = "Impecable")] Mint,
    [Display(Name = "Bueno")] Good,
    [Display(Name = "Regular")] Fair,
    [Display(Name = "Malo")] Poor
}

public enum TradeType
{
    [Display(Name = "Venta")] Sale,
    [Display(Name = "Intercambio")] Trade,
    [Display(Name = "Venta e Intercambio")] Both
}

public enum TradeStatus
{
    [Display(Name = "Activo")] Active,
    [Display(Name = "Pendiente")] Pending,
    [Display(Name = "Cerrado")] Closed,
    [Display(Name = "Cancelado")] Cancelled
}
