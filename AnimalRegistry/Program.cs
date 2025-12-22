using AnimalRegistry.Data;
using AnimalRegistry.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Dodanie us³ug (Kontrolery + Widoki)
builder.Services.AddControllersWithViews();

// 2. Konfiguracja bazy danych SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=animalregistry.db"));

// 3. Konfiguracja Systemu Identity (U¿ytkownicy i Role)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    // Tutaj mo¿esz zmieniæ wymagania co do has³a
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 4. Konfiguracja przekierowañ logowania
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

// 5. SEED - Automatyczne tworzenie ról i konta urzêdnika
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // Tworzymy role (Zwróæ uwagê na wielkie litery - musz¹ byæ takie same jak w kontrolerze!)
    string[] roles = new[] { "Hodowca", "Urzednik" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Tworzymy konto testowe urzêdnika
    var adminEmail = "urzednik@test.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(admin, "Test123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Urzednik");
        }
    }
}

// 6. Konfiguracja potoku ¿¹dañ (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Bardzo wa¿ne: Authentication musi byæ PRZED Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();