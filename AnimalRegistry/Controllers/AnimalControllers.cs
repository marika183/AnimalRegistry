using AnimalRegistry.Data;
using AnimalRegistry.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalRegistry.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AnimalsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. LISTA AKTYWNYCH ZWIERZĄT (Z SORTOWANIEM)
        public async Task<IActionResult> Index(string sortOrder)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            // Przygotowanie parametrów dla linków w nagłówkach
            ViewData["CurrentSort"] = sortOrder;
            ViewData["SpeciesSortParm"] = String.IsNullOrEmpty(sortOrder) ? "species_desc" : "";
            ViewData["NumberSortParm"] = sortOrder == "Number" ? "number_desc" : "Number";
            ViewData["OwnerSortParm"] = sortOrder == "Owner" ? "owner_desc" : "Owner";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            var query = _context.Animals
                .Include(a => a.Owner)
                .Where(a => a.Status != AnimalStatus.Zarchiwizowany);

            // Filtrowanie pod rolę
            bool isUrzednik = await _userManager.IsInRoleAsync(user, "Urzednik") ||
                              await _userManager.IsInRoleAsync(user, "Urzędnik");

            if (!isUrzednik)
            {
                query = query.Where(a => a.OwnerId == user.Id);
            }

            // Logika sortowania
            query = sortOrder switch
            {
                "species_desc" => query.OrderByDescending(a => a.Species),
                "Number" => query.OrderBy(a => a.IdentificationNumber),
                "number_desc" => query.OrderByDescending(a => a.IdentificationNumber),
                "Owner" => query.OrderBy(a => a.Owner.UserName),
                "owner_desc" => query.OrderByDescending(a => a.Owner.UserName),
                "Status" => query.OrderBy(a => a.Status),
                "status_desc" => query.OrderByDescending(a => a.Status),
                _ => query.OrderBy(a => a.Species),
            };

            return View(await query.ToListAsync());
        }

        // 2. LISTA ZARCHIWIZOWANYCH (Z SORTOWANIEM)
        public async Task<IActionResult> ArchivedIndex(string sortOrder)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isUrzednik = await _userManager.IsInRoleAsync(user, "Urzednik") ||
                              await _userManager.IsInRoleAsync(user, "Urzędnik");

            if (!isUrzednik) return Forbid();

            // Parametry sortowania dla archiwum
            ViewData["NumberSortParm"] = String.IsNullOrEmpty(sortOrder) ? "number_desc" : "";
            ViewData["OwnerSortParm"] = sortOrder == "Owner" ? "owner_desc" : "Owner";

            var archivedQuery = _context.Animals
                .Include(a => a.Owner)
                .Where(a => a.Status == AnimalStatus.Zarchiwizowany);

            // Logika sortowania
            archivedQuery = sortOrder switch
            {
                "number_desc" => archivedQuery.OrderByDescending(a => a.IdentificationNumber),
                "Owner" => archivedQuery.OrderBy(a => a.Owner.UserName),
                "owner_desc" => archivedQuery.OrderByDescending(a => a.Owner.UserName),
                _ => archivedQuery.OrderBy(a => a.IdentificationNumber),
            };

            return View(await archivedQuery.ToListAsync());
        }

        // 3. SZCZEGÓŁY I HISTORIA
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals
                .Include(a => a.Owner)
                .Include(a => a.History)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (animal == null) return NotFound();

            return View(animal);
        }

        // 4. REJESTRACJA (GET)
        public IActionResult Create()
        {
            return View(new Animal());
        }

        // 5. REJESTRACJA (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Animal animal, IFormFile? pdfFile)
        {
            ModelState.Remove("Owner");
            ModelState.Remove("OwnerId");
            ModelState.Remove("History");

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                animal.OwnerId = user.Id;
                animal.Status = AnimalStatus.Oczekujacy;

                if (pdfFile != null && pdfFile.Length > 0)
                {
                    animal.VeterinaryCertificateUrl = await SaveFile(pdfFile);
                }

                _context.Animals.Add(animal);
                await _context.SaveChangesAsync();

                _context.AnimalHistories.Add(new AnimalHistory
                {
                    AnimalId = animal.Id,
                    ChangeDescription = "Zarejestrowano zwierzę z dokumentem weterynaryjnym.",
                    ChangeDate = DateTime.Now
                });
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }

        // 6. EDYCJA (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            if (animal.Status == AnimalStatus.Zarchiwizowany)
            {
                TempData["ErrorMessage"] = "Nie można edytować zarchiwizowanego rekordu.";
                return RedirectToAction(nameof(Index));
            }

            return View(animal);
        }

        // 7. EDYCJA (POST)
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, Animal animal, IFormFile? pdfFile)
{
    if (id != animal.Id) return NotFound();

    var original = await _context.Animals.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    if (original == null) return NotFound();

    var user = await _userManager.GetUserAsync(User);
    bool isUrzednik = await _userManager.IsInRoleAsync(user, "Urzednik") ||
                      await _userManager.IsInRoleAsync(user, "Urzędnik");

    // Przypisanie stałych wartości
    animal.OwnerId = original.OwnerId;
    if (!isUrzednik) animal.Status = original.Status;

    // BLOKADA DANYCH DLA ZATWIERDZONYCH (jeśli nie jesteś urzędnikiem)
    if (original.Status == AnimalStatus.Zatwierdzony && !isUrzednik)
    {
        animal.Species = original.Species;
        animal.IdentificationNumber = original.IdentificationNumber;
        animal.DateOfBirth = original.DateOfBirth;
        animal.CountryOfOrigin = original.CountryOfOrigin;
        animal.Source = original.Source; // Dodałem blokadę źródła
    }

    ModelState.Remove("Owner");
    ModelState.Remove("OwnerId");
    ModelState.Remove("History");

    if (ModelState.IsValid)
    {
        try
        {
            List<string> changes = new List<string>();

            // Obsługa pliku PDF
            if (pdfFile != null && pdfFile.Length > 0)
            {
                animal.VeterinaryCertificateUrl = await SaveFile(pdfFile);
                changes.Add("Zaktualizowano dokument PDF");
            }
            else
            {
                animal.VeterinaryCertificateUrl = original.VeterinaryCertificateUrl;
            }

            // LOGIKA SPRAWDZANIA ZMIAN (Dodałem Source i CountryOfOrigin)
            if (original.Species != animal.Species) changes.Add($"Gatunek: '{original.Species}' -> '{animal.Species}'");
            if (original.IdentificationNumber != animal.IdentificationNumber) changes.Add($"Nr ident.: '{original.IdentificationNumber}' -> '{animal.IdentificationNumber}'");
            if (original.DateOfBirth.Date != animal.DateOfBirth.Date) changes.Add($"Data ur.: {original.DateOfBirth.ToShortDateString()} -> {animal.DateOfBirth.ToShortDateString()}");
            if (original.DateOfDeath != animal.DateOfDeath) changes.Add($"Data zgonu: {(original.DateOfDeath?.ToShortDateString() ?? "brak")} -> {(animal.DateOfDeath?.ToShortDateString() ?? "brak")}");
            if (original.Status != animal.Status) changes.Add($"Status: {original.Status} -> {animal.Status}");
            
            // NOWE LINIE - Bez nich się nie zapisze!
            if (original.CountryOfOrigin != animal.CountryOfOrigin) changes.Add($"Kraj: '{original.CountryOfOrigin}' -> '{animal.CountryOfOrigin}'");
            if (original.Source != animal.Source) changes.Add($"Źródło: '{original.Source}' -> '{animal.Source}'");

            if (changes.Any())
            {
                _context.Update(animal);
                _context.AnimalHistories.Add(new AnimalHistory
                {
                    AnimalId = animal.Id,
                    ChangeDescription = $"Edycja ({user.UserName}): " + string.Join(", ", changes),
                    ChangeDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Animals.Any(e => e.Id == animal.Id)) return NotFound();
            else throw;
        }
        return RedirectToAction(nameof(Index));
    }
    return View(animal);
}

        // 8. ARCHIWIZACJA
        public async Task<IActionResult> Archive(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            bool isUrzednik = await _userManager.IsInRoleAsync(user, "Urzednik") ||
                              await _userManager.IsInRoleAsync(user, "Urzędnik");

            if (!isUrzednik) return Forbid();

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null) return NotFound();

            animal.Status = AnimalStatus.Zarchiwizowany;

            _context.AnimalHistories.Add(new AnimalHistory
            {
                AnimalId = animal.Id,
                ChangeDescription = "Zwierzę zostało zarchiwizowane.",
                ChangeDate = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

            string filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }
    }
}