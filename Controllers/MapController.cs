using GameVault.Data;
using GameVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

public class MapController : Controller
{
    private readonly ApplicationDbContext _context;

    public MapController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var collectors = await _context.Users
            .Where(u => u.Latitude != null && u.Longitude != null)
            .Select(u => new MapCollectorViewModel
            {
                UserId = u.Id,
                DisplayName = u.DisplayName,
                Latitude = u.Latitude!.Value,
                Longitude = u.Longitude!.Value,
                City = u.City,
                ActiveAssetCount = u.Assets.Count(a => a.IsActive),
            })
            .ToListAsync();

        return View(collectors);
    }
}
