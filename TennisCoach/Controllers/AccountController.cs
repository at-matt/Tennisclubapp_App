using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

using TennisCoach.Data;
using TennisCoach.Models; // Make sure to include this for your view models
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TennisCoach.Migrations;

namespace TennisCoach.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger; // Declare the logger

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
            _logger = logger;
        }
        private int GetMemberId()
        {
            // Assuming you have a way to get the member ID
            var userEmail = User.Identity.Name; // Get the logged-in user's email
            var member = _context.Members.FirstOrDefault(m => m.Email == userEmail); // Adjust based on your logic
            return member?.MemberId ?? 0; // Return MemberId or 0 if not found
        }



        private static readonly List<Admins> Admins = new List<Admins>
    {
        new Admins { Username = "admin1", Password = "password1" },
        new Admins { Username = "admin2", Password = "password2" }
    };

        [HttpPost]
        public IActionResult Login(AdminLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the entered credentials match the hardcoded admins
                var admin = Admins.FirstOrDefault(a => a.Username == model.Username && a.Password == model.Password);
                if (admin != null)
                {
                    // Successful login logic
                    // Example: Set a session variable or cookie
                    HttpContext.Session.SetString("AdminUsername", admin.Username);
                    return RedirectToAction("Index", "Admin"); // Redirect to the home page after successful login
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }
            return View(model); // Return the view with validation errors if login fails
        }
        public IActionResult LoginAdmin()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUsername"); // Clear the session
            return RedirectToAction("LoginAdmin");
        }





        //GET: Coach/LoginCoach

        public IActionResult LoginCoach()
        {
            return View();
        }
        [HttpPost]
        [Route("Account/LoginCoach")]
        public async Task<IActionResult> LoginCoach(CoachLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string trimmedEmail = model.Email.Trim();
                string trimmedPassword = model.Password.Trim();

                if (string.IsNullOrEmpty(trimmedEmail) || string.IsNullOrEmpty(trimmedPassword))
                {
                    ModelState.AddModelError("", "Email and Password cannot be empty.");
                    return View(model);  // Return to login view with validation messages
                }

                var coach = await _context.Coaches
                    .FirstOrDefaultAsync(m => m.Email == trimmedEmail);

                if (coach != null)
                {
                    bool isPasswordValid = coach.Password == trimmedPassword;

                    if (isPasswordValid)
                    {
                        // Optional: Handle null fields if you need to access them
                        string firstName = coach.FirstName ?? "No First Name"; // Handle null
                        string lastName = coach.LastName ?? "No Last Name"; // Handle null
                        string biography = coach.Biography ?? "No Biography"; // Handle null
                        byte[] photo = coach.Photo; // This can remain null

                        var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, coach.CoachId.ToString()),
                    new Claim(ClaimTypes.Email, coach.Email),
                    new Claim("FirstName", firstName), // Add First Name to claims
                    new Claim("LastName", lastName)    // Add Last Name to claims
                    // Add more claims if necessary
                };

                        var identity = new ClaimsIdentity(claims, "LoginCoach");
                        var principal = new ClaimsPrincipal(identity);

                        await HttpContext.SignInAsync(principal);

                        return RedirectToAction("Index", "Coach");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid password.");
                        return RedirectToAction("ErrorMessage", "Error");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No Coach found with this email.");
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }

            return View(model);  // Return to login view with validation messages
        }



        // GET: Register
        public IActionResult RegCoach()
        {
            return View();
        }
        // POST: Register Coach
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegCoach(CoachRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if a Coach with the provided email already exists
                var existingCoach = await _context.Coaches
                    .FirstOrDefaultAsync(m => m.Email.Trim() == model.Email.Trim());

                if (existingCoach == null)
                {
                    // Create a new Coach without hashing the password (NOT recommended for security)
                    var newCoach = new Coaches
                    {
                        Email = model.Email.Trim(),
                        Password = model.Password // Storing the plain password (not secure)
                    };

                    // Add the new Coach to the database
                    _context.Coaches.Add(newCoach);
                    await _context.SaveChangesAsync(); // Save changes to the database

                    // Redirect to the login page
                    return RedirectToAction("LoginCoach", "Account");
                }
                else
                {
                    // Coach with the email already exists
                    ModelState.AddModelError(string.Empty, "A Coach with this email already exists.");
                }
            }

            return View(model); // Return the view with validation errors if any
        }











        // GET: Member/Login
        public IActionResult LoginMember()
        {
            return View();
        }


        // POST: Member/Login
        [HttpPost]
        [Route("Account/LoginMember")]
        public async Task<IActionResult> LoginMember(MemberLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Email.Trim() == model.Email.Trim());

                if (member != null)
                {
                    // Use BCrypt to verify the password
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, member.Password);

                    if (isPasswordValid)
                    {
                        // Create claims for the logged-in member
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, member.MemberId.ToString()),
                            new Claim(ClaimTypes.Name, member.FirstName + " " + member.LastName)
                        };

                        var identity = new ClaimsIdentity(claims, "LoginMember");
                        var principal = new ClaimsPrincipal(identity);

                        // Perform sign-in
                        await HttpContext.SignInAsync(principal);

                        return RedirectToAction("Index", "Member");  // Redirect to home or any other page after login
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid password.");
                        return RedirectToAction("ErrorMessage", "Error");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "No member found with this email.");
                    return RedirectToAction("ErrorMessage", "Error");
                }
            }

            return View(model);  // Return to login view with validation messages
        }

        // GET: Register
        public IActionResult RegMember()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegMember(MemberRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if a member with the provided email already exists
                var existingMember = await _context.Members
                    .FirstOrDefaultAsync(m => m.Email.Trim() == model.Email.Trim());

                if (existingMember == null)
                {
                    // Hash the password
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    // Create a new member with the hashed password
                    var newMember = new Members
                    {
                        FirstName = model.FirstName.Trim(),
                        LastName = model.LastName.Trim(),
                        Email = model.Email.Trim(),
                        Password = hashedPassword,  // Store hashed password
                        Active = true // New members are active by default
                    };

                    // Add the new member to the database
                    _context.Members.Add(newMember);
                    await _context.SaveChangesAsync(); // Save changes to the database

                    // Redirect to the login page
                 
                    return RedirectToAction("LoginMember", "Account");
                }
                else
                {
                    // Member with the email already exists
                    ModelState.AddModelError(string.Empty, "A member with this email already exists.");
                }
            }

            return View(model); // Return the view with validation errors if any


        }


    }


}

