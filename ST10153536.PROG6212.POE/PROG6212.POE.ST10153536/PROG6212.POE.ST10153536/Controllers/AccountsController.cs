using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROG6212.POE.ST10153536.Interfaces;
using PROG6212.POE.ST10153536.Models;
using System.Security.Claims;

public class AccountsController : Controller
{
    private readonly IUserService _userService;
    private readonly ApplicationDbContext _context;

    public AccountsController(IUserService userService, ApplicationDbContext context) 
    {
        _userService = userService;
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Register(Users user)
    {
        try
        {
            var registrationResult = _userService.RegisterUser(user);

            if (registrationResult.Success)
            {
                TempData["SuccessMessage"] = "User successfully registered and saved to the database.";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, registrationResult.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "An error occurred during registration.");
        }

        return View(user);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(string username, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new { success = false });
            }

            // Assuming GetUserByUsername returns Users type
            var user = _userService.GetUserByUsername(username) as Users;

            if (user != null)
            {
                // Capture the user ID
                var userId = user.UserId;

                // Display the user ID in the console
                Console.WriteLine($"User logged in. UserId: {userId}");

                // Check if a record for the user exists in Modules table
                var userModules = _context.Modules.Where(m => m.UserId == userId).ToList();

                if (userModules.Count == 0)
                {
                    // default values
                    var emptyModule = new Modules
                    {
                        UserId = userId,
                        ModuleId = 2,
                        Code = "prog",
                        Name = "DefaultName",
                        Credits = 0,
                        ClassHoursPerWeek = 0,
                        NumberOfWeeks = 0,
                        StartDate = DateTime.Now,
                        HoursSpent = 0,
                        SelfStudyHours = 0,
                        CurrentWeekSelfStudyRemain = 0
                    };

                    _context.Modules.Add(emptyModule);
                    _context.SaveChanges();
                    Console.WriteLine($"Created an empty module record for UserId: {userId}");
                }

                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {

                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                string redirectUrl = Url.Action("ModuleShow", "Module");
                return Json(new { success = true, redirectUrl, userId });
            }

            return Json(new { success = false });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            return Json(new { success = false });
        }
    }



}


