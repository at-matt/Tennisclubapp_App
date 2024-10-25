using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TennisCoach.Data;
using TennisCoach.Models;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TennisCoach.Migrations;

namespace TennisCoach.Controllers
{
    public class MemberController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MemberController(ApplicationDbContext context)
        {
            _context = context;

        }

        private int GetMemberId()
        {
            var userEmail = User.Identity.Name;
            var member = _context.Members.FirstOrDefault(m => m.Email == userEmail);
            return member?.MemberId ?? 0; // Return MemberId or 0 if not found
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult EnrolledCoaches()
        {
            var memberId = GetMemberId(); // Retrieve the current member's ID

            // Get the list of coaches for the current member based on their enrollments
            var enrolledCoaches = _context.Enrollments
                                          .Include(e => e.Schedule)
                                          .ThenInclude(s => s.Coach) // Ensure coach details are included
                                          .Where(e => e.MemberId == memberId)
                                          .Select(e => e.Schedule.Coach) // Get the coaches from the schedules
                                          .Distinct() // In case of duplicate coach entries
                                          .ToList();

            return View(enrolledCoaches);
        }
        public IActionResult Coach()
        {
            return View();
        }


        public async Task<IActionResult> Coaches()
        {
            // Retrieve the list of coaches
            var coaches = await _context.Coaches.ToListAsync();
            // Log the count of retrieved coaches for debugging
            Console.WriteLine($"Coaches retrieved: {coaches.Count}");
            // Pass the list of coaches to the view
            return View(coaches);
        }



        public async Task<IActionResult> Enroll()
        {
            // Retrieve the list of schedules, including the coach information
            var schedules = await _context.Schedules
                                          .Include(s => s.Coach) // Include the related Coach entity
                                          .ToListAsync();

            // Pass the list of schedules to the view
            return View(schedules);
        }


        [HttpPost]
        public async Task<IActionResult> Enroll(int scheduleId)
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (memberIdClaim == null)
            {
                return Unauthorized("User is not logged in.");
            }

            int memberId = int.Parse(memberIdClaim.Value);
            var existingEnrollment = await _context.Enrollments
       .FirstOrDefaultAsync(e => e.MemberId == memberId && e.ScheduleId == scheduleId);

            if (existingEnrollment != null)
            {
                TempData["ErrorMessage"] = "You are already enrolled in this schedule.";
                return RedirectToAction("Index"); // Redirect to the appropriate view
            }

            var enrollment = new Enrollments
            {
                MemberId = memberId,
                ScheduleId = scheduleId
            };

            // Add enrollment to the database
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            // Optionally, provide feedback
            TempData["SuccessMessage"] = "You have successfully enrolled in the schedule!";
            return RedirectToAction("Index"); // Redirect to the appropriate view
        }

        public async Task<IActionResult> EnrolledSchedules()
        {
            var memberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (memberIdClaim == null)
            {
                return Unauthorized("User is not logged in.");
            }

            int memberId = int.Parse(memberIdClaim.Value);

            // Fetch enrolled schedules for the member
            var enrolledSchedules = await _context.Enrollments
                .Include(e => e.Schedule)  // Include the Schedule details
                .Where(e => e.MemberId == memberId)
                .ToListAsync();

            return View(enrolledSchedules);  // Return the list to the view
        }



    }
}







