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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlTypes;

namespace TennisCoach.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger; // Declare the logger

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        // GET: Admin/ViewMembers
        public IActionResult ViewMemberships()
        {
            // Fetch all enrollments including related schedules, members, and coaches
            var enrollments = _context.Enrollments
                .Include(e => e.Schedule)
                .ThenInclude(s => s.Coach) // Include Coach details
                .Include(e => e.Member)    // Include Member details
                .Select(e => new Schedules
                {
                    ScheduleId = e.ScheduleId,
                    EventName = e.Schedule.EventName,
                    Date = e.Schedule.Date,
                    Location = e.Schedule.Location,
                    Coach = new Coaches
                    {
                        FirstName = e.Schedule.Coach.FirstName,
                        LastName = e.Schedule.Coach.LastName
                    },
                    MemberId = e.MemberId,
                    Member = new Members
                    {
                        FirstName = e.Member.FirstName,
                        LastName = e.Member.LastName
                    }
                })
                .ToList();

            return View(enrollments);
        }

        // GET: CreateSchedule
        public IActionResult CreateSchedule()
        {
            // Retrieve distinct CoachIds using Entity Framework
            var coachIds = _context.Coaches
                .Select(c => c.CoachId)
                .Distinct()
                .ToList();

            // Create SelectListItems for the dropdown
            List<SelectListItem> coachItems = coachIds
                .Select(id => new SelectListItem { Value = id.ToString(), Text = id.ToString() })
                .ToList();

            ViewData["CoachIds"] = coachItems;

            // Return the CreateSchedule view with an empty model
            return View(new CreateSchedule());
        }

        // POST: CreateSchedule
        [HttpPost]
        public IActionResult CreateSchedule(CreateSchedule model)
        {
            if (ModelState.IsValid)
            {
                // Map the CreateSchedule model to a Schedules entity
                Schedules schedule = new Schedules
                {
                    CoachId = model.CoachId,
                    Date = model.Date,
                    EventName = model.EventName,
                    Location = model.Location,
                    Description = model.Description
                    // Map other fields as needed
                };

                // Add the new schedule using Entity Framework
                _context.Schedules.Add(schedule);
                _context.SaveChanges();
                return RedirectToAction("Index"); // Redirect to Index after creation
            }

            // If the model state is invalid, retrieve the coach list again for the dropdown
            var coachIds = _context.Coaches
                .Select(c => c.CoachId)
                .Distinct()
                .ToList();

            ViewData["CoachIds"] = coachIds
                .Select(id => new SelectListItem { Value = id.ToString(), Text = id.ToString() })
                .ToList();

            // Return the view with the current model state (CreateSchedule model)
            return View(model);
        }




















        // GET: Admin/MatchCoach
        public IActionResult MatchCoach()
        {
            var schedules = _context.Schedules.ToList();
            var coaches = _context.Coaches.ToList();

            List<SelectListItem> scheduleItems = schedules
                .Select(s => new SelectListItem { Value = s.ScheduleId.ToString(), Text = s.ScheduleId.ToString() })
                .ToList();

            List<SelectListItem> coachItems = coaches
                .Select(c => new SelectListItem { Value = c.CoachId.ToString(), Text = c.CoachId.ToString() })
                .ToList();

            ViewData["ScheduleIds"] = scheduleItems;
            ViewData["CoachIds"] = coachItems;

            return View(new Schedules());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MatchCoach(Schedules sch)
        {
            var scheduleToUpdate = _context.Schedules.Find(sch.ScheduleId);
            if (scheduleToUpdate != null)
            {
                scheduleToUpdate.CoachId = sch.CoachId; // Assuming you have a CoachId property in your Schedules model
                _context.SaveChanges(); // Save changes to the database

                return RedirectToAction("Index");




            }
            
            return RedirectToAction("Index");

            // Redirect to appropriate views based on matching result


        }
    }

}


