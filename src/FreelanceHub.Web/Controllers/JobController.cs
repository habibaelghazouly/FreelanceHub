using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Web.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly ApplicationDbContext _dbContext;

        public JobController(IJobService jobService, ApplicationDbContext dbContext)
        {
            _jobService = jobService;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create()
        {
            // Load categories, tags, and skills from database
            var categories = await _dbContext.Categories.ToListAsync();
            var tags = await _dbContext.Tags.ToListAsync();
            var skills = await _dbContext.Skills.ToListAsync();

            // Create view model with modal data
            var viewModel = new CreateJobViewModel();
            ViewBag.Categories = categories.Select(c => new SelectableItem { Id = c.CategoryId, Name = c.Name }).ToList();
            ViewBag.Tags = tags.Select(t => new SelectableItem { Id = t.TagId, Name = t.Name }).ToList();
            ViewBag.Skills = skills.Select(s => new SelectableItem { Id = s.SkillId, Name = s.Name }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(CreateJobViewModel model)
        {
            Console.WriteLine("Model State: " + ModelState.IsValid);
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _jobService.CreateJobAsync(ToCreateJobRequest(model, int.Parse(userId)), HttpContext.RequestAborted);
            // if (!result.Succeeded)
            // {
            //     AddErrors(result);
            //     return View(model);
            // }

            return RedirectToAction("Index", "Home");
        }
        public CreateJobRequest ToCreateJobRequest(CreateJobViewModel model , int userId)
        {
            return new CreateJobRequest
            {
                Title = model.Title,
                Description = model.Description,
                Budget = model.Budget,
                Deadline = model.Deadline,
                ClientId = userId,
                CategoryIds = model.CategoryIds,
                SkillIds = model.SkillIds,
                TagIds = model.TagIds
            };
        }

        public IActionResult Index()
        {
            return View();
        }

        
    }
}