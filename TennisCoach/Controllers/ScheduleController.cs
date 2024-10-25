using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using TennisCoach.Models;
using TennisCoach.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TennisCoach.Migrations;

public class SchedulesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public SchedulesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        // Get the logged-in user's member ID (assume it's stored in the user's claim or profile)
        var memberId = GetMemberId(); // This method would retrieve the actual member ID

        // Filter schedules based on the logged-in member and include coach information
        var schedules = _context.Schedules
                                .Include(s => s.Coach) // Include coach information first
                                .Where(s => s.MemberId == memberId && s.Date >= DateTime.Now) // Then filter by member ID and date
                                .ToList();

        return View(schedules);
    }


    private int GetMemberId()
    {
        var userEmail = User.Identity.Name; // Assuming the email is used as the identity name
        var member = _context.Members.FirstOrDefault(m => m.Email == userEmail);
        return member?.MemberId ?? 0; // Return MemberId or 0 if not found
    }
    private int GetCurrentMemberId()
    {
        // Assuming you are using a Claims-based authentication system
        var memberIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        return memberIdClaim != null ? int.Parse(memberIdClaim.Value) : 0; // Return 0 or a default value if not found
    }


}

