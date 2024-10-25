using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using TennisCoach.Data;
using TennisCoach.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public class EnrollmentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Members> _userManager;

    public EnrollmentController(ApplicationDbContext context, UserManager<Members> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public async Task<IActionResult> EnrolledSchedules()
    {
        var memberId = GetMemberId(); // Retrieve the logged-in member's ID

        if (memberId <= 0)
        {
            return RedirectToAction("Login", "Account"); // Redirect to login if not logged in
        }

        // Fetch schedules that the member is enrolled in
        var enrolledSchedules = await _context.Enrollments
            .Where(e => e.MemberId == memberId)
            .Include(e => e.Schedule) // Include related Schedule data
            .ThenInclude(s => s.Coach) // Optionally include related Coach data if needed
            .ToListAsync();

        return View(enrolledSchedules); // Pass the enrolled schedules to the view
    }




    public async Task<IActionResult> Index()
    {
        var memberId = GetMemberId(); // Get the current member's ID

        if (memberId <= 0)
        {
            return RedirectToAction("LoginMember", "Account"); // Redirect if not logged in
        }

        var enrollments = await _context.Enrollments
            .Include(e => e.Schedule) // Include schedule details
            .ThenInclude(s => s.Coach) // Include coach details if necessary
            .Where(e => e.MemberId == memberId)
            .ToListAsync();

        return View(enrollments); // Pass the enrolled schedules to the view
    }
    private int GetMemberId()
    {
        var userEmail = User.Identity.Name; // Getting the logged-in user's email
        var member = _context.Members.FirstOrDefault(m => m.Email == userEmail);
        return member?.MemberId ?? 0; // Return MemberId or 0 if not found
    }


}
