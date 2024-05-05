using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212.POE.ST10153536.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class ModuleController : Controller
{
    private readonly ApplicationDbContext _context;
    private double CalculateCurrentWeekHoursLeft(Modules module)
    {
        double totalStudyHoursPerWeek = module.ClassHoursPerWeek + CalculateSelfStudyHoursAsDouble(module);

        double remainingHours = totalStudyHoursPerWeek - (module.HoursSpent ?? 0);

        remainingHours = Math.Round(remainingHours, 2);

        return remainingHours;
    }

    private double CalculateSelfStudyHoursAsDouble(Modules module)
    {
        double calculatedSelfStudyHours = (module.NumberOfWeeks - module.ClassHoursPerWeek != 0)
            ? (module.Credits * 10.0) / (module.NumberOfWeeks - module.ClassHoursPerWeek)
            : 0.0;

        return calculatedSelfStudyHours;
    }
    private double CalculateCurrentHoursLeft(Modules module)
    {
        double remainingHours = (double)(module.SelfStudyHours - (module.HoursSpent ?? 0));

        remainingHours = Math.Max(remainingHours, 0);

        remainingHours = Math.Round(remainingHours, 2);

        return remainingHours;
    }


    public ModuleController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> ModuleShow()
    {
        var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

        var modules = await _context.Modules
            .Where(m => m.UserId == userId)
            .ToListAsync();

        foreach (var module in modules)
        {
            module.HoursSpent = module.HoursSpent ?? 0;
        }

        return View(modules);
    }




    private double? FormatRemainingHours(double? remainingHours)
    {
        if (remainingHours.HasValue)
        {
            int hours = (int)remainingHours.Value;
            int minutes = (int)((remainingHours.Value - hours) * 60);

            if (hours == 0 && minutes == 0)
            {
                return 0.0;
            }

            return double.Parse($"{hours}.{minutes}");
        }

        return null;
    }

    // GET: Module/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.ModuleId == id);

        if (module == null)
        {
            return NotFound();
        }

        return View(module);
    }
    // GET: Module/Create
    public IActionResult Create()
    {
        try
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

                Console.WriteLine($"Entering Create action (GET) - UserId: {userId}");

                var module = new Modules
                {
                    UserId = userId,
                    Code = "DefaultCode",
                    Name = "DefaultName",
                    Credits = 0,
                    ClassHoursPerWeek = 0,
                    NumberOfWeeks = 0,
                    StartDate = DateTime.Now
                };

                module.CurrentWeekSelfStudyRemain = CalculateCurrentWeekHoursLeft(module);

                if (module.HoursSpent != null)
                {
                    module.CurrentWeekSelfStudyRemain -= module.HoursSpent.Value;
                }

                module.SelfStudyHours = module.CurrentWeekSelfStudyRemain;

                return View(module);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred in the Create action (GET): {ex.Message}");
            return RedirectToAction("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Code,Name,Credits,ClassHoursPerWeek,NumberOfWeeks,StartDate")] Modules module)
    {
        Console.WriteLine("Entering Create action (POST)");

        if (User.Identity.IsAuthenticated)
        {
            try
            {
                if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                {
                    module.UserId = userId;
                }

                if (module.ModuleId == 0)
                {
                    Console.WriteLine("Creating a new module.");

                    module.ModuleId = GenerateRandomModuleId();
                    Console.WriteLine($"Generated ModuleId: {module.ModuleId}");

                    double calculatedSelfStudyHours = Math.Round(module.Credits * 10.0 / (module.NumberOfWeeks - module.ClassHoursPerWeek), 2);

                    module.SelfStudyHours = calculatedSelfStudyHours;
                    Console.WriteLine($"Calculated SelfStudyHours: {module.SelfStudyHours}");

                    module.CurrentWeekSelfStudyRemain = calculatedSelfStudyHours;
                    Console.WriteLine($"CurrentWeekSelfStudyRemain: {module.CurrentWeekSelfStudyRemain}");
                    _context.Add(module);
                    Console.WriteLine("Module added to the context.");

                    await _context.SaveChangesAsync();
                    Console.WriteLine($"Module saved successfully. ModuleId: {module.ModuleId}, SelfStudyHours: {module.SelfStudyHours}");
                }

                return RedirectToAction(nameof(ModuleShow));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the module: {ex.Message}");
            }
        }

        return RedirectToAction("Login");
    }

    private double? CalculateSelfStudyHours(Modules module)
    {
        double calculatedSelfStudyHours = (module.NumberOfWeeks - module.ClassHoursPerWeek != 0)
            ? (module.Credits * 10.0) / (module.NumberOfWeeks - module.ClassHoursPerWeek)
            : 0.0;

        return calculatedSelfStudyHours;
    }

    private int GenerateRandomModuleId()
    {

        Random random = new Random();
        return random.Next(1, 100);
    }

    // GET: Module/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var module = await _context.Modules.FindAsync(id);

        if (module == null)
        {
            return NotFound();
        }

        return View(module);
    }
    // POST: Module/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ModuleId,Code,Name,Credits,ClassHoursPerWeek,NumberOfWeeks,StartDate,HoursSpent")] Modules module)
    {
        Console.WriteLine($"Entering Edit action (POST) for module ID: {id}");

        if (ModelState.IsValid)
        {
            try
            {
                Console.WriteLine("ModelState is valid.");

                module.UserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                var existingModule = await _context.Modules.FindAsync(id);

                if (existingModule == null)
                {
                    return NotFound();
                }

                Console.WriteLine($"Preserving existing SelfStudyHours: {existingModule.SelfStudyHours}");
                module.SelfStudyHours = existingModule.SelfStudyHours;

                Console.WriteLine($"Calculating CurrentWeekSelfStudyRemain...");
                module.CurrentWeekSelfStudyRemain = CalculateCurrentHoursLeft(module);
                Console.WriteLine($"Calculated CurrentWeekSelfStudyRemain: {module.CurrentWeekSelfStudyRemain}");

                Console.WriteLine("Updating properties of the existing module...");
                _context.Entry(existingModule).CurrentValues.SetValues(module);

                module.CurrentWeekSelfStudyRemain = module.SelfStudyHours;

                module.CurrentWeekSelfStudyRemain -= module.HoursSpent ?? 0;

                double totalStudyHours = module.ClassHoursPerWeek + CalculateSelfStudyHoursAsDouble(module);
                double totalHours = totalStudyHours * (module.NumberOfWeeks * 7);  // Assuming 7 days in a week
                module.CurrentWeekSelfStudyRemain = CalculateCurrentHoursLeft(module);

                Console.WriteLine("Saving changes to the database...");
                await _context.SaveChangesAsync();

                Console.WriteLine("Module updated successfully.");

                return RedirectToAction(nameof(ModuleShow));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the module: {ex.Message}");
                return RedirectToAction("Error");
            }
        }

        foreach (var modelState in ModelState.Values)
        {
            foreach (var error in modelState.Errors)
            {
                Console.WriteLine($"Model error: {error.ErrorMessage}");
            }
        }

        return View(module);
    }

    // GET: Module/Delete/5
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var module = await _context.Modules.FindAsync(id);

        if (module == null)
        {
            return NotFound();
        }

        return View(module);
    }

    // POST: Module/Delete/5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var module = await _context.Modules.FindAsync(id);
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine($"ModelState error: {error.ErrorMessage}");
            }

            return BadRequest(ModelState);
        }
        if (module != null)
        {
            _context.Modules.Remove(module);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(ModuleShow));
    }

    private bool ModuleExists(int id)
    {
        return _context.Modules.Any(e => e.ModuleId == id);
    }
}