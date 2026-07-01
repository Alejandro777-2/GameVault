using System.Diagnostics;
using GameVault.Data;
using GameVault.Models;
using GameVault.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameVault.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new HomeIndexViewModel
        {
            TotalAssets = await _context.Assets.CountAsync(a => a.IsActive),
            TotalCollectors = await _context.Users.CountAsync(),
            TotalPlatforms = await _context.Assets
                .Where(a => a.IsActive)
                .Select(a => a.Platform)
                .Distinct()
                .CountAsync()
        };
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
