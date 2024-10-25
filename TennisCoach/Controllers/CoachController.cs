using Microsoft.AspNetCore.Mvc;
using TennisCoach.Data;
using TennisCoach.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TennisCoach.Migrations;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace TennisCoach.Controllers
{
    public class CoachController : Controller
    {
        private readonly ApplicationDbContext _context;




        public CoachController(ApplicationDbContext context)
        {
            _context = context;
        }
        private int GetCurrentCoachId()
        {
            var CoachIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get CoachId from logged-in user
            if (string.IsNullOrEmpty(CoachIdClaim))
            {
                return 0; // or throw an exception, or handle it as per your application's needs
            }

            // Parse the CoachIdClaim to an integer and return it
            if (int.TryParse(CoachIdClaim, out int CoachId))
            {
                return CoachId;
            }

            return 0; // If parsing fails, return 0 or handle accordingly
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: Coaches/EnrolledCoaches
        public IActionResult EnrolledCoaches()
        {
            var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if memberId is null or empty
            if (string.IsNullOrEmpty(memberId))
            {
                return Unauthorized();
            }

            // Parse the memberId safely
            if (!int.TryParse(memberId, out int memberIdParsed))
            {
                return BadRequest("Invalid member ID.");
            }

            // Fetch enrolled coaches for the logged-in member
            var enrolledCoaches = _context.Enrollments
                .Include(e => e.Schedule)
                .Include(e => e.Schedule.Coach) // Include the coach for each schedule
                .Where(e => e.MemberId == memberIdParsed)
                .Select(e => e.Schedule.Coach)
                .Distinct() // Ensure coaches are unique
                .ToList();

            if (enrolledCoaches.Any())
            {
                return View(enrolledCoaches); // Return the view with the list of enrolled coaches
            }

            return View("NoEnrolledCoaches"); // Show a different view if no coaches are found
        }


        public IActionResult Details()
        {
            var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if CoachId is null or empty
            if (string.IsNullOrEmpty(CoachId))
            {
                return Unauthorized();
            }

            // Parse the CoachId safely
            if (!int.TryParse(CoachId, out int coachIdParsed))
            {
                return BadRequest("Invalid coach ID.");
            }

            // Fetch the coach from the database
            var Coach = _context.Coaches.SingleOrDefault(c => c.CoachId == coachIdParsed);

            // Check if Coach is found
            if (Coach == null)
            {
                return NotFound();
            }

            // Create CoachProfileViewModel with null checks
            var CoachProfileViewModel = new CoachProfileViewModel
            {
                FirstName = Coach.FirstName?.Trim() ?? string.Empty, // Use null-coalescing operator
                LastName = Coach.LastName?.Trim() ?? string.Empty,
                Email = Coach.Email?.Trim() ?? string.Empty,
                Biography = Coach.Biography,
                Photo = Coach.Photo
            };

            return View(Coach); // Return the ViewModel instead of the Coach object
        }




        // GET: Coach/Create
        [HttpGet]
        public IActionResult CreateProfile()
        {
            var userId = GetCurrentCoachId();
            // Log the userId for debugging
            Console.WriteLine($"Current Coach ID: {userId}");

            if (userId == 0)
            {
                return NotFound();
            }

            var coach = _context.Coaches.FirstOrDefault(c => c.CoachId == userId);
            if (coach == null)
            {
                return NotFound();
            }

            return View(coach);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(Coaches coach)
        {
            // Get the user ID using UserManager
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's ID
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); // or handle it accordingly
            }

            // Convert userId to int if necessary
            int parsedUserId;
            if (!int.TryParse(userId, out parsedUserId))
            {
                return BadRequest("Invalid user ID.");
            }

            // Find the coach in the database
            var coachToUpdate = await _context.Coaches.FindAsync(parsedUserId);
            if (coachToUpdate == null)
            {
                return NotFound();
            }

            // Update the coach's details
            coachToUpdate.FirstName = coach.FirstName;
            coachToUpdate.LastName = coach.LastName;
            coachToUpdate.Biography = coach.Biography;

            // Handle photo upload
            if (coach.PhotoFile != null && coach.PhotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await coach.PhotoFile.CopyToAsync(memoryStream);
                    coachToUpdate.Photo = memoryStream.ToArray();
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = coachToUpdate.CoachId });
        }




        [HttpGet]
        [Route("Coach/EditProfile/{id}")]
        public IActionResult EditProfile(int id)
        {
            var coach = _context.Coaches.Find(id);
            if (coach == null)
            {
                return NotFound();
            }

            var model = new EditCoachProfileViewModel
            {
                CoachId = coach.CoachId,
                FirstName = coach.FirstName,
                LastName = coach.LastName,
                Biography = coach.Biography
                // Populate other fields as necessary
            };

            return View(model);
        }

        [HttpPost]
        [Route("Coach/EditProfile/{id}")]
        public IActionResult EditProfile(EditCoachProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingCoach = _context.Coaches.FirstOrDefault(c => c.CoachId == model.CoachId);
            if (existingCoach == null)
            {
                return NotFound();
            }

            existingCoach.FirstName = model.FirstName;
            existingCoach.LastName = model.LastName;
            existingCoach.Biography = model.Biography;

            if (model.PhotoFile != null && model.PhotoFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    model.PhotoFile.CopyTo(memoryStream);
                    existingCoach.Photo = memoryStream.ToArray(); // Assuming Photo is byte[]
                }
            }

            _context.SaveChanges();

            return RedirectToAction("Details", new { id = existingCoach.CoachId });
        }






        // GET: Coaches/UpcomingSchedules
        public IActionResult UpcomingSchedules()
        {
            var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if CoachId is null or empty
            if (string.IsNullOrEmpty(CoachId))
            {
                return Unauthorized();
            }

            // Parse the CoachId safely
            if (!int.TryParse(CoachId, out int coachIdParsed))
            {
                return BadRequest("Invalid coach ID.");
            }

            // Fetch the coach from the database
            var Coach = _context.Coaches.SingleOrDefault(c => c.CoachId == coachIdParsed);

            // Check if Coach is found
            if (Coach == null)
            {
                return NotFound();
            }

            // Fetch upcoming schedules for the logged-in coach
            var upcomingSchedules = _context.Schedules
                .Include(s => s.Coach)
                .Where(s => s.CoachId == coachIdParsed && s.Date > DateTime.Now) // Only select future schedules
                .Select(s => new Schedules
                {
                    ScheduleId = s.ScheduleId,
                    Date = s.Date,
                    Location = s.Location,
                    Description = s.Description,
                    Coach = new Coaches
                    {
                        FirstName = s.Coach.FirstName,
                        LastName = s.Coach.LastName,
                        CoachId = s.CoachId
                    }
                })
                .ToList();

            if (upcomingSchedules.Any())
            {
                return View(upcomingSchedules);
            }

            return View("NoUpcomingSchedules"); // Show a different view if no schedules are found
        }




        // GET: Coaches/EnrolledMembers
        public IActionResult EnrolledMembers()
        {
            var CoachId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if CoachId is null or empty
            if (string.IsNullOrEmpty(CoachId))
            {
                return Unauthorized();
            }

            // Parse the CoachId safely
            if (!int.TryParse(CoachId, out int coachIdParsed))
            {
                return BadRequest("Invalid coach ID.");
            }

            // Fetch the coach from the database
            var Coach = _context.Coaches.SingleOrDefault(c => c.CoachId == coachIdParsed);

            // Check if Coach is found
            if (Coach == null)
            {
                return NotFound();
            }

            // Fetch enrolled members for the schedules coached by the active coach
            var enrolledMembers = _context.Enrollments
                .Include(e => e.Member) // Include related Member entity
                .Include(e => e.Schedule) // Include related Schedule entity
                .Where(e => e.Schedule.CoachId == coachIdParsed && e.Schedule.Date > DateTime.Now)
                .Select(e => new Members
                {
                    MemberId = e.Member.MemberId,
                    FirstName = e.Member.FirstName,
                    LastName = e.Member.LastName,
                    Email = e.Member.Email,

                })
                .ToList();

            // Check if there are any enrolled members
            if (enrolledMembers.Any())
            {
                return View(enrolledMembers);
            }

            return View("NoEnrolledMembers");
        }

    }
}