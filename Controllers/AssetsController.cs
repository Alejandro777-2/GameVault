using GameVault.Data;
using GameVault.Helpers;
using GameVault.Models;
using GameVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

public class AssetsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWebHostEnvironment _env;
    private const int PageSize = 12;

    public AssetsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IWebHostEnvironment env)
    {
        _context = context;
        _userManager = userManager;
        _env = env;
    }

    // GET: /Assets
    [AllowAnonymous]
    public async Task<IActionResult> Index(
        string? searchTerm, Platform? platform, Condition? condition, int page = 1)
    {
        var query = _context.Assets
            .Include(a => a.Owner)
            .Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(a => EF.Functions.Like(a.Title, $"%{searchTerm}%"));

        if (platform.HasValue)
            query = query.Where(a => a.Platform == platform.Value);

        if (condition.HasValue)
            query = query.Where(a => a.Condition == condition.Value);

        query = query.OrderByDescending(a => a.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var assets = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var vm = new AssetIndexViewModel
        {
            Assets = assets.Select(a => new AssetCardViewModel
            {
                Id = a.Id,
                Title = a.Title,
                Platform = a.Platform,
                Year = a.Year,
                Condition = a.Condition,
                EstimatedValue = a.EstimatedValue,
                ImageUrl = a.ImageUrl,
                OwnerDisplayName = a.Owner.DisplayName,
                OwnerId = a.OwnerId,
                OfferType = a.OfferType,
                TradeWants = a.TradeWants,
            }).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalItems,
            SearchTerm = searchTerm,
            PlatformFilter = platform,
            ConditionFilter = condition,
        };

        return View(vm);
    }

    // GET: /Assets/Details/{id}
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var asset = await _context.Assets
            .Include(a => a.Owner)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        if (asset == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        bool isInWishlist = currentUserId != null
            && await _context.WishlistItems
                .AnyAsync(w => w.UserId == currentUserId && w.AssetId == id);

        bool hasActiveTradeOffer = await _context.TradeOffers
            .AnyAsync(t => t.AssetId == id && t.Status == TradeStatus.Active);

        return View(new AssetDetailsViewModel
        {
            Asset = asset,
            OwnerDisplayName = asset.Owner.DisplayName,
            OwnerCity = asset.Owner.City,
            OwnerCountry = asset.Owner.Country,
            IsCurrentUserOwner = currentUserId == asset.OwnerId,
            IsInWishlist = isInWishlist,
            HasActiveTradeOffer = hasActiveTradeOffer,
        });
    }

    // GET: /Assets/Create
    [Authorize]
    public IActionResult Create() => View(new AssetFormViewModel());

    // POST: /Assets/Create
    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Create(AssetFormViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        string? imageUrl = null;
        if (vm.ImageFile != null)
        {
            if (!ValidateImage(vm.ImageFile)) return View(vm);
            imageUrl = await SaveImageAsync(vm.ImageFile);
        }

        var asset = new Asset
        {
            Title = vm.Title,
            Platform = vm.Platform,
            Year = vm.Year,
            Region = vm.Region,
            Condition = vm.Condition,
            Description = vm.Description,
            EstimatedValue = vm.EstimatedValue,
            OfferType = vm.OfferType,
            TradeWants = vm.OfferType == TradeType.Sale ? null : vm.TradeWants,
            ImageUrl = imageUrl,
            OwnerId = _userManager.GetUserId(User)!,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Activo registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // GET: /Assets/Edit/{id}
    [Authorize]
    public async Task<IActionResult> Edit(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null || !asset.IsActive) return NotFound();
        if (asset.OwnerId != _userManager.GetUserId(User)) return Forbid();

        return View(new AssetFormViewModel
        {
            Id = asset.Id,
            Title = asset.Title,
            Platform = asset.Platform,
            Year = asset.Year,
            Region = asset.Region,
            Condition = asset.Condition,
            Description = asset.Description,
            EstimatedValue = asset.EstimatedValue,
            OfferType = asset.OfferType,
            TradeWants = asset.TradeWants,
            CurrentImageUrl = asset.ImageUrl,
        });
    }

    // POST: /Assets/Edit/{id}
    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Edit(int id, AssetFormViewModel vm)
    {
        if (id != vm.Id) return BadRequest();

        var asset = await _context.Assets.FindAsync(id);
        if (asset == null || !asset.IsActive) return NotFound();
        if (asset.OwnerId != _userManager.GetUserId(User)) return Forbid();

        if (!ModelState.IsValid)
        {
            vm.CurrentImageUrl = asset.ImageUrl;
            return View(vm);
        }

        if (vm.ImageFile != null)
        {
            if (!ValidateImage(vm.ImageFile))
            {
                vm.CurrentImageUrl = asset.ImageUrl;
                return View(vm);
            }
            DeleteImageFile(asset.ImageUrl);
            asset.ImageUrl = await SaveImageAsync(vm.ImageFile);
        }

        asset.Title = vm.Title;
        asset.Platform = vm.Platform;
        asset.Year = vm.Year;
        asset.Region = vm.Region;
        asset.Condition = vm.Condition;
        asset.Description = vm.Description;
        asset.EstimatedValue = vm.EstimatedValue;
        asset.OfferType = vm.OfferType;
        asset.TradeWants = vm.OfferType == TradeType.Sale ? null : vm.TradeWants;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Activo actualizado correctamente.";
        return RedirectToAction(nameof(Details), new { id = asset.Id });
    }

    // GET: /Assets/Delete/{id}
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var asset = await _context.Assets
            .Include(a => a.Owner)
            .FirstOrDefaultAsync(a => a.Id == id && a.IsActive);

        if (asset == null) return NotFound();
        if (asset.OwnerId != _userManager.GetUserId(User)) return Forbid();

        return View(asset);
    }

    // POST: /Assets/Delete/{id}
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null || !asset.IsActive) return NotFound();
        if (asset.OwnerId != _userManager.GetUserId(User)) return Forbid();

        asset.IsActive = false;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Activo eliminado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    // ---- private helpers ----

    private bool ValidateImage(IFormFile file)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowed.Contains(ext))
        {
            ModelState.AddModelError(nameof(AssetFormViewModel.ImageFile),
                "Solo se permiten imágenes en formato JPG, PNG o WEBP.");
            return false;
        }

        if (file.Length > 5 * 1024 * 1024)
        {
            ModelState.AddModelError(nameof(AssetFormViewModel.ImageFile),
                "La imagen no puede superar los 5 MB.");
            return false;
        }

        return true;
    }

    private async Task<string> SaveImageAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var dir = Path.Combine(_env.WebRootPath, "uploads", "assets");
        Directory.CreateDirectory(dir);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var fullPath = Path.Combine(dir, fileName);
        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);
        return $"/uploads/assets/{fileName}";
    }

    private void DeleteImageFile(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return;
        var path = Path.Combine(_env.WebRootPath,
            imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (System.IO.File.Exists(path))
            System.IO.File.Delete(path);
    }
}
