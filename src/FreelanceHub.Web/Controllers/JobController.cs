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
            var userId = ((ClaimsIdentity)User.Identity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                CreateJobRequest request = ToCreateJobRequest(model, int.Parse(userId));
                Console.WriteLine($"Request: Title={request.Title}, Description={request.Description}, Budget={request.Budget}, Deadline={request.Deadline}, ClientId={request.ClientId}, CategoryIds={request.CategoryIds}, SkillIds={request.SkillIds}, TagIds={request.TagIds}");
                Console.WriteLine($"JobFiles Count: {request.JobFiles.Count}");
                var result = await _jobService.CreateJobAsync(request, HttpContext.RequestAborted);

            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("MyJobs");
        }
        public CreateJobRequest ToCreateJobRequest(CreateJobViewModel model, int userId)
        {
            var openedStreams = new List<Stream>();
            var jobFiles = new List<UploadedFileRequest>();

            Console.WriteLine($"Model: Title={model.Title}, Description={model.Description}, Budget={model.Budget}, Deadline={model.Deadline}, ClientId={userId}, CategoryIds={model.CategoryIds}, SkillIds={model.SkillIds}, TagIds={model.TagIds}");
            Console.WriteLine($"JobFiles Count: {model.JobFiles.Count}");
            try
            {
                foreach (var jobFile in model.JobFiles.Where(file => file.Length > 0))
                {
                    Console.WriteLine($"Processing file: {jobFile.FileName}, Size: {jobFile.Length}, ContentType: {jobFile.ContentType}");
                    var stream = jobFile.OpenReadStream();
                    openedStreams.Add(stream);
                    jobFiles.Add(new UploadedFileRequest(stream, jobFile.FileName, jobFile.ContentType, jobFile.Length));
                }
            }
            catch
            {
                // Handle any exceptions that may occur while opening the streams
                foreach (var stream in openedStreams)
                {
                    stream.Dispose();
                }
                throw;
            }
            return new CreateJobRequest
            {
                Title = model.Title,
                Description = model.Description,
                Budget = model.Budget,
                Deadline = model.Deadline,
                ClientId = userId,
                CategoryIds = model.CategoryIds ?? string.Empty,
                SkillIds = model.SkillIds ?? string.Empty,
                TagIds = model.TagIds ?? string.Empty,
                JobFiles = jobFiles
            };
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MyJobs()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var jobs = await _jobService.GetJobsByClientIdAsync(userId);

            return View(jobs);
        }

        public async Task<IActionResult> Index()
        {
            var jobs = await _jobService.GetAllJOpeningJobsAsync();

            return View(jobs);
        }

        public async Task<IActionResult> DetailJob(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }
    }
}