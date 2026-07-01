using GameVault.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameVault.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            bool hasAssets    = context.Assets.Any();
            bool hasWishlists = context.WishlistItems.Any();

            if (hasAssets && hasWishlists)
                return;

            // ── Users ──────────────────────────────────────────────────────
            var carlos = await GetOrCreateUserAsync(userManager, new ApplicationUser
            {
                Email = "carlos.retrohunter@gamevault.demo",
                UserName = "carlos.retrohunter@gamevault.demo",
                DisplayName = "Carlos el Retrohunter",
                City = "Quito",
                Country = "Ecuador",
                ReputationScore = 87,
                EmailConfirmed = true
            }, "Demo123!");

            var maria = await GetOrCreateUserAsync(userManager, new ApplicationUser
            {
                Email = "maria.pixelqueen@gamevault.demo",
                UserName = "maria.pixelqueen@gamevault.demo",
                DisplayName = "María PixelQueen",
                City = "Guayaquil",
                Country = "Ecuador",
                ReputationScore = 124,
                EmailConfirmed = true
            }, "Demo123!");

            var andres = await GetOrCreateUserAsync(userManager, new ApplicationUser
            {
                Email = "andres.8bitmaster@gamevault.demo",
                UserName = "andres.8bitmaster@gamevault.demo",
                DisplayName = "Andrés 8BitMaster",
                City = "Cuenca",
                Country = "Ecuador",
                ReputationScore = 56,
                EmailConfirmed = true
            }, "Demo123!");

            // ── Assets ─────────────────────────────────────────────────────
            if (!hasAssets)
            {
                var now = DateTime.UtcNow;

                context.Assets.AddRange(new List<Asset>
                {
                    new() {
                        Title = "Super Mario Bros.",
                        Platform = Platform.NES,
                        Year = 1985,
                        Region = Region.NTSC_US,
                        Condition = Condition.Mint,
                        EstimatedValue = 145m,
                        OwnerId = carlos.Id,
                        Description = "Cartucho original con etiqueta intacta, en perfecto estado de conservación. Pieza icónica del lanzamiento del NES, imprescindible en cualquier colección seria.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-43)
                    },
                    new() {
                        Title = "The Legend of Zelda: Ocarina of Time",
                        Platform = Platform.N64,
                        Year = 1998,
                        Region = Region.NTSC_US,
                        Condition = Condition.Mint,
                        EstimatedValue = 185m,
                        OwnerId = maria.Id,
                        Description = "Edición dorada en estado impecable, una de las joyas más buscadas de la era N64. Sin un solo rasguño en el cartucho ni en los contactos.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-38)
                    },
                    new() {
                        Title = "Chrono Trigger",
                        Platform = Platform.SNES,
                        Year = 1995,
                        Region = Region.NTSC_JP,
                        Condition = Condition.Mint,
                        EstimatedValue = 290m,
                        OwnerId = andres.Id,
                        Description = "Importación japonesa con caja y manual perfectos, sin marcas de uso visibles. JRPG legendario de Square, considerado uno de los mejores juegos de todos los tiempos.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-35)
                    },
                    new() {
                        Title = "Final Fantasy VII",
                        Platform = Platform.PS1,
                        Year = 1997,
                        Region = Region.PAL,
                        Condition = Condition.Good,
                        EstimatedValue = 95m,
                        OwnerId = carlos.Id,
                        Description = "Los tres discos completos y funcionales, acompañados del manual original con leves marcas de uso. Edición PAL europea en excelente estado general.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-30)
                    },
                    new() {
                        Title = "Pokémon Red",
                        Platform = Platform.GameBoy,
                        Year = 1996,
                        Region = Region.NTSC_US,
                        Condition = Condition.Fair,
                        EstimatedValue = 65m,
                        OwnerId = maria.Id,
                        Description = "Cartucho completamente funcional con la batería interna todavía operativa. La etiqueta presenta una ligera decoloración propia del paso del tiempo.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-27)
                    },
                    new() {
                        Title = "Super Metroid",
                        Platform = Platform.SNES,
                        Year = 1994,
                        Region = Region.NTSC_US,
                        Condition = Condition.Mint,
                        EstimatedValue = 210m,
                        OwnerId = andres.Id,
                        Description = "Estado de exhibición, prácticamente sin uso aparente. Una de las piezas más cotizadas del catálogo SNES por su narrativa atmosférica y jugabilidad.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-24)
                    },
                    new() {
                        Title = "Metal Gear Solid",
                        Platform = Platform.PS1,
                        Year = 1998,
                        Region = Region.NTSC_US,
                        Condition = Condition.Good,
                        EstimatedValue = 115m,
                        OwnerId = carlos.Id,
                        Description = "Ambos discos en perfecto estado, con manual y caja original. Se aprecian escasas marcas menores en la portada, pero nada que afecte la presentación.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-21)
                    },
                    new() {
                        Title = "GoldenEye 007",
                        Platform = Platform.N64,
                        Year = 1997,
                        Region = Region.PAL,
                        Condition = Condition.Fair,
                        EstimatedValue = 58m,
                        OwnerId = maria.Id,
                        Description = "Cartucho funcional sin ningún problema de lectura, con desgaste menor en la etiqueta. Se vende sin caja; ideal para completar una biblioteca N64.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-18)
                    },
                    new() {
                        Title = "Resident Evil 2",
                        Platform = Platform.PS1,
                        Year = 1998,
                        Region = Region.NTSC_US,
                        Condition = Condition.Mint,
                        EstimatedValue = 155m,
                        OwnerId = andres.Id,
                        Description = "Edición original de doble disco con el sello prácticamente intacto. Uno de los survival horror más emblemáticos de la era PS1, en condición excepcional.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-15)
                    },
                    new() {
                        Title = "Castlevania: Symphony of the Night",
                        Platform = Platform.PS1,
                        Year = 1997,
                        Region = Region.NTSC_US,
                        Condition = Condition.Good,
                        EstimatedValue = 130m,
                        OwnerId = carlos.Id,
                        Description = "El disco está impecable y funciona a la perfección; la caja presenta una ligera abolladura en una esquina que no afecta el interior. Clásico absoluto del género metroidvania.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-12)
                    },
                    new() {
                        Title = "Earthbound",
                        Platform = Platform.SNES,
                        Year = 1995,
                        Region = Region.NTSC_US,
                        Condition = Condition.Fair,
                        EstimatedValue = 235m,
                        OwnerId = maria.Id,
                        Description = "El cartucho gigante original acompañado de su caja, sin la guía estratégica. Uno de los títulos más raros y cotizados del catálogo SNES.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-8)
                    },
                    new() {
                        Title = "Sonic Adventure",
                        Platform = Platform.Dreamcast,
                        Year = 1998,
                        Region = Region.NTSC_US,
                        Condition = Condition.Good,
                        EstimatedValue = 78m,
                        OwnerId = andres.Id,
                        Description = "Pieza nostálgica de la última consola de Sega, en buen estado de conservación. El GD-ROM está sin rayones y el case original se encuentra completo.",
                        IsActive = true,
                        ImageUrl = null,
                        CreatedAt = now.AddDays(-4)
                    },
                });

                await context.SaveChangesAsync();
            }

            // ── Wishlist items ─────────────────────────────────────────────
            if (!hasWishlists)
            {
                // Re-fetch user IDs in case they were pre-existing
                var carlosId = (await userManager.FindByEmailAsync("carlos.retrohunter@gamevault.demo"))!.Id;
                var mariaId  = (await userManager.FindByEmailAsync("maria.pixelqueen@gamevault.demo"))!.Id;
                var andresId = (await userManager.FindByEmailAsync("andres.8bitmaster@gamevault.demo"))!.Id;

                // Map titles to IDs
                var assetIds = await context.Assets
                    .Where(a => a.IsActive)
                    .Select(a => new { a.Id, a.Title, a.OwnerId })
                    .ToListAsync();

                int? IdOf(string title) => assetIds.FirstOrDefault(a => a.Title == title)?.Id;

                var wishlistSeed = new List<(string UserId, string Title)>
                {
                    // Carlos saves 3 items from María and Andrés
                    (carlosId, "The Legend of Zelda: Ocarina of Time"),
                    (carlosId, "Chrono Trigger"),
                    (carlosId, "Earthbound"),
                    // María saves 3 items from Carlos and Andrés
                    (mariaId,  "Super Mario Bros."),
                    (mariaId,  "Super Metroid"),
                    (mariaId,  "Resident Evil 2"),
                    // Andrés saves 2 items from Carlos and María
                    (andresId, "Final Fantasy VII"),
                    (andresId, "Pokémon Red"),
                };

                var now = DateTime.UtcNow;
                int offset = 0;
                foreach (var (userId, title) in wishlistSeed)
                {
                    var assetId = IdOf(title);
                    if (assetId == null) continue;

                    // Skip if the asset belongs to the user (safety check)
                    var ownerId = assetIds.First(a => a.Id == assetId).OwnerId;
                    if (ownerId == userId) continue;

                    context.WishlistItems.Add(new WishlistItem
                    {
                        UserId  = userId,
                        AssetId = assetId.Value,
                        AddedAt = now.AddDays(-(offset++)),
                    });
                }

                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DbSeeder] Error al sembrar datos: {ex.Message}");
        }
    }

    private static async Task<ApplicationUser> GetOrCreateUserAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string password)
    {
        var existing = await userManager.FindByEmailAsync(user.Email!);
        if (existing != null)
            return existing;

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception($"No se pudo crear el usuario {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        return user;
    }
}
