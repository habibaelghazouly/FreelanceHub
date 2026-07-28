using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FreelanceHub.Web.Controllers
{
    [Authorize]
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IJobRepository _jobRepository;

        public JobController(IJobService jobService, IJobRepository jobRepository)
        {
            _jobService = jobService;
            _jobRepository = jobRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create()
        {
            await PopulateCreateOptionsAsync();
            return View(new CreateJobViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create(CreateJobViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                await PopulateCreateOptionsAsync();
                return View(model);
            }
            if (!int.TryParse(userId, out var clientId))
            {
                return Forbid();
            }

            var request = ToCreateJobRequest(model, clientId);
            CreateJobResult result;
            try
            {
                result = await _jobService.CreateJobAsync(request, HttpContext.RequestAborted);
            }
            finally
            {
                foreach (var file in request.JobFiles) file.Content.Dispose();
            }
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
                await PopulateCreateOptionsAsync();
                return View(model);
            }

            return RedirectToAction("MyJobs");
        }
        public CreateJobRequest ToCreateJobRequest(CreateJobViewModel model, int userId)
        {
            var openedStreams = new List<Stream>();
            var jobFiles = new List<UploadedFileRequest>();

            try
            {
                foreach (var jobFile in (model.JobFiles ?? []).Where(file => file.Length > 0))
                {
                    var stream = jobFile.OpenReadStream();
                    openedStreams.Add(stream);
                    jobFiles.Add(new UploadedFileRequest(stream, jobFile.FileName, jobFile.ContentType, jobFile.Length));
                }
            }
            catch
            {
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
                JobFiles = jobFiles ?? new List<UploadedFileRequest>()
            };
        }

        [Authorize(Roles = "Client")]
        public async Task<IActionResult> MyJobs()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var jobs = await _jobService.GetJobsByClientIdAsync(userId);

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

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Browse(JobBrowseViewModel model)
        {
            if (model.MaxBudget is < 0) ModelState.AddModelError(nameof(model.MaxBudget), "Maximum budget cannot be negative.");
            if (model.PageNumber < 1) model.PageNumber = 1;
            model.PageSize = Math.Clamp(model.PageSize, 1, 100);
            if (model.SortOrder is not ("date" or "budget")) model.SortOrder = "date";

            var result = await _jobRepository.BrowseOpenAsync(model.CategoryId, model.MaxBudget, model.SkillId, model.SortOrder, model.PageNumber, model.PageSize, HttpContext.RequestAborted);
            model.TotalCount = result.TotalCount;
            model.Jobs = result.Jobs;
            model.Categories = (await _jobRepository.ListCategoriesAsync(HttpContext.RequestAborted)).Select(category => new SelectListItem(category.Name, category.CategoryId.ToString())).ToList();
            model.Skills = (await _jobRepository.ListSkillsAsync(HttpContext.RequestAborted)).Select(skill => new SelectListItem(skill.Name, skill.SkillId.ToString())).ToList();
            return View("~/Views/Jobs/Browse.cshtml", model);
        }

        private async Task PopulateCreateOptionsAsync()
        {
            ViewBag.Categories = (await _jobRepository.ListCategoriesAsync(HttpContext.RequestAborted)).Select(category => new SelectableItem { Id = category.CategoryId, Name = category.Name }).ToList();
            ViewBag.Tags = (await _jobRepository.ListTagsAsync(HttpContext.RequestAborted)).Select(tag => new SelectableItem { Id = tag.TagId, Name = tag.Name }).ToList();
            ViewBag.Skills = (await _jobRepository.ListSkillsAsync(HttpContext.RequestAborted)).Select(skill => new SelectableItem { Id = skill.SkillId, Name = skill.Name }).ToList();
        }
    }
}
