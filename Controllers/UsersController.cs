using GameVault.Data;
using GameVault.Models;
using GameVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

public class UsersController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [AllowAnonymous]
    public async Task<IActionResult> Profile(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound();

        var recentAssets = await _context.Assets
            .Where(a => a.OwnerId == id && a.IsActive)
            .OrderByDescending(a => a.CreatedAt)
            .Take(12)
            .Select(a => new AssetCardViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Platform = a.Platform,
                Year = a.Year,
                Condition = a.Condition,
                EstimatedValue = a.EstimatedValue,
                ImageUrl = a.ImageUrl,
                OwnerId = id,
                OwnerDisplayName = user.DisplayName
            })
            .ToListAsync();

        var vm = new UserProfileViewModel
        {
            User = user,
            RecentAssets = recentAssets,
            TotalActiveAssets = await _context.Assets.CountAsync(a => a.OwnerId == id && a.IsActive)
        };

        return View(vm);
    }
}
