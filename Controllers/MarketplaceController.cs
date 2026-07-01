using GameVault.Data;
using GameVault.Models;
using GameVault.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

public class MarketplaceController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private const int PageSize = 12;

    public MarketplaceController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: /Marketplace
    [AllowAnonymous]
    public async Task<IActionResult> Index(Platform? platform, TradeType? type, int page = 1)
    {
        var query = _context.TradeOffers
            .Include(t => t.Asset)
            .Include(t => t.Owner)
            .Where(t => t.Status == TradeStatus.Active && t.Asset.IsActive);

        if (platform.HasValue)
            query = query.Where(t => t.Asset.Platform == platform.Value);

        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        query = query.OrderByDescending(t => t.CreatedAt);

        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var offers = await query
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var vm = new MarketplaceIndexViewModel
        {
            Offers = offers.Select(t => new TradeOfferCardViewModel
            {
                Id = t.Id,
                AssetId = t.AssetId,
                AssetTitle = t.Asset.Title,
                AssetImageUrl = t.Asset.ImageUrl,
                AssetPlatform = t.Asset.Platform,
                AssetCondition = t.Asset.Condition,
                Type = t.Type,
                Price = t.Price,
                OwnerDisplayName = t.Owner.DisplayName,
                OwnerId = t.OwnerId,
                CreatedAt = t.CreatedAt,
            }).ToList(),
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalItems,
            PlatformFilter = platform,
            TypeFilter = type,
        };

        return View(vm);
    }

    // GET: /Marketplace/Details/{id}
    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var offer = await _context.TradeOffers
            .Include(t => t.Asset)
            .Include(t => t.Owner)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (offer == null) return NotFound();

        return View(new MarketplaceDetailsViewModel
        {
            Offer = offer,
            OwnerEmail = offer.Owner.Email ?? string.Empty,
        });
    }

    // GET: /Marketplace/Create?assetId={id}
    [Authorize]
    public async Task<IActionResult> Create(int assetId)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == assetId && a.IsActive);

        if (asset == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User);
        if (asset.OwnerId != currentUserId) return Forbid();

        bool hasActiveOffer = await _context.TradeOffers
            .AnyAsync(t => t.AssetId == assetId && t.Status == TradeStatus.Active);

        if (hasActiveOffer)
        {
            TempData["ErrorMessage"] = "Este activo ya tiene una oferta activa en el marketplace.";
            return RedirectToAction("Details", "Assets", new { id = assetId });
        }

        return View(new TradeOfferFormViewModel
        {
            AssetId = assetId,
            AssetTitle = asset.Title,
            AssetImageUrl = asset.ImageUrl,
            AssetPlatform = asset.Platform,
            AssetYear = asset.Year,
            AssetCondition = asset.Condition,
        });
    }

    // POST: /Marketplace/Create
    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Create(TradeOfferFormViewModel vm)
    {
        var asset = await _context.Assets
            .FirstOrDefaultAsync(a => a.Id == vm.AssetId && a.IsActive);

        if (asset == null) return NotFound();

        var currentUserId = _userManager.GetUserId(User)!;
        if (asset.OwnerId != currentUserId) return Forbid();

        if (vm.Type != TradeType.Trade && vm.Price == null)
            ModelState.AddModelError(nameof(vm.Price), "El precio es obligatorio para ofertas de venta.");

        bool hasActiveOffer = await _context.TradeOffers
            .AnyAsync(t => t.AssetId == vm.AssetId && t.Status == TradeStatus.Active);

        if (hasActiveOffer)
            ModelState.AddModelError(string.Empty, "Este activo ya tiene una oferta activa en el marketplace.");

        if (!ModelState.IsValid)
        {
            vm.AssetTitle = asset.Title;
            vm.AssetImageUrl = asset.ImageUrl;
            vm.AssetPlatform = asset.Platform;
            vm.AssetYear = asset.Year;
            vm.AssetCondition = asset.Condition;
            return View(vm);
        }

        _context.TradeOffers.Add(new TradeOffer
        {
            AssetId = vm.AssetId,
            OwnerId = currentUserId,
            Type = vm.Type,
            Price = vm.Type == TradeType.Trade ? null : vm.Price,
            Description = vm.Description,
            Status = TradeStatus.Active,
            CreatedAt = DateTime.UtcNow,
        });

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Oferta publicada en el marketplace correctamente.";
        return RedirectToAction(nameof(MyOffers));
    }

    // GET: /Marketplace/MyOffers
    [Authorize]
    public async Task<IActionResult> MyOffers()
    {
        var currentUserId = _userManager.GetUserId(User);

        var offers = await _context.TradeOffers
            .Include(t => t.Asset)
            .Where(t => t.OwnerId == currentUserId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        return View(offers);
    }

    // POST: /Marketplace/Cancel/{id}
    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Cancel(int id)
    {
        var offer = await _context.TradeOffers.FindAsync(id);

        if (offer == null) return NotFound();
        if (offer.OwnerId != _userManager.GetUserId(User)) return Forbid();

        if (offer.Status != TradeStatus.Active)
        {
            TempData["ErrorMessage"] = "Solo se pueden cancelar ofertas activas.";
            return RedirectToAction(nameof(MyOffers));
        }

        offer.Status = TradeStatus.Cancelled;
        offer.ClosedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Oferta cancelada correctamente.";
        return RedirectToAction(nameof(MyOffers));
    }
}
