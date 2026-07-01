using GameVault.Data;
using GameVault.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

[Authorize]
public class WishlistController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET /Wishlist
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User)!;

        var items = await _context.WishlistItems
            .Include(w => w.Asset)
                .ThenInclude(a => a.Owner)
            .Where(w => w.UserId == userId && w.Asset.IsActive)
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();

        return View(items);
    }

    // POST /Wishlist/Add
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int assetId)
    {
        var userId = _userManager.GetUserId(User)!;

        var asset = await _context.Assets.FindAsync(assetId);
        if (asset == null || !asset.IsActive)
        {
            TempData["ErrorMessage"] = "El activo no existe o no está disponible.";
            return RedirectToAction("Index", "Assets");
        }

        if (asset.OwnerId == userId)
        {
            TempData["ErrorMessage"] = "No puedes guardar tus propios activos en la wishlist.";
            return RedirectToAction("Details", "Assets", new { id = assetId });
        }

        bool alreadySaved = await _context.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.AssetId == assetId);

        if (alreadySaved)
        {
            TempData["ErrorMessage"] = "Este activo ya está en tu wishlist.";
            return RedirectToAction("Details", "Assets", new { id = assetId });
        }

        _context.WishlistItems.Add(new WishlistItem
        {
            UserId = userId,
            AssetId = assetId,
            AddedAt = DateTime.UtcNow,
        });
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Activo guardado en tu wishlist.";
        return RedirectToAction("Details", "Assets", new { id = assetId });
    }

    // POST /Wishlist/Remove/{id}
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id)
    {
        var userId = _userManager.GetUserId(User)!;

        var item = await _context.WishlistItems.FindAsync(id);
        if (item == null || item.UserId != userId)
            return NotFound();

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Removido de tu wishlist.";
        return RedirectToAction(nameof(Index));
    }
}
