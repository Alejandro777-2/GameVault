using GameVault.Data;
using GameVault.Models;
using GameVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

public class CollectorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public CollectorsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var usersWithAssets = await _context.Users
            .Where(u => u.Assets.Any(a => a.IsActive))
            .Select(u => new
            {
                User = u,
                ActiveAssets = u.Assets.Where(a => a.IsActive).ToList(),
            })
            .ToListAsync();

        var collectors = usersWithAssets.Select(x => new CollectorMapEntry
        {
            User = x.User,
            ActiveAssetCount = x.ActiveAssets.Count,
            TopPlatforms = x.ActiveAssets
                .GroupBy(a => a.Platform)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList(),
        })
        .OrderByDescending(e => e.ActiveAssetCount)
        .ToList();

        // Map markers are a subset of the same collectors list — only those with geocoded coordinates.
        // This guarantees the two panels are always consistent (same universe of users).
        var mapMarkers = collectors
            .Where(e => e.User.Latitude.HasValue && e.User.Longitude.HasValue)
            .Select(e => new MapCollectorViewModel
            {
                UserId = e.User.Id,
                DisplayName = e.User.DisplayName,
                Latitude = e.User.Latitude!.Value,
                Longitude = e.User.Longitude!.Value,
                City = e.User.City,
                ActiveAssetCount = e.ActiveAssetCount,
            })
            .ToList();

        return View(new ComunidadIndexViewModel
        {
            Collectors = collectors,
            MapMarkers = mapMarkers,
        });
    }
}
