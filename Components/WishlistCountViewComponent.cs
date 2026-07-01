using GameVault.Data;
using GameVault.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Components;

public class WishlistCountViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public WishlistCountViewComponent(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!UserClaimsPrincipal.Identity?.IsAuthenticated ?? true)
            return Content(string.Empty);

        var userId = _userManager.GetUserId(UserClaimsPrincipal);
        if (userId == null)
            return Content(string.Empty);

        var count = await _context.WishlistItems
            .CountAsync(w => w.UserId == userId && w.Asset.IsActive);

        return View(count);
    }
}
