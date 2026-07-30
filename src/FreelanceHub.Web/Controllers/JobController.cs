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

        public JobController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> Create()
        {
            await PopulateCreateOptionsAsync(true);
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
                await PopulateCreateOptionsAsync(true);
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
                await PopulateCreateOptionsAsync(true);
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
        [AllowAnonymous]
        public async Task<IActionResult> Browse(JobBrowseViewModel model)
        {
            var result = await _jobService.BrowseJobsAsync(
                model.CategoryId,
                model.SkillId,
                model.MaxBudget,
                model.SortOrder,
                model.PageNumber,
                model.PageSize,
                HttpContext.RequestAborted);
            var data = await _jobService.GetCreateJobPageDataAsync();
            return View(Mapper(result, model,data));
        }

        private JobBrowseViewModel Mapper(BrowseJobsResult result, JobBrowseViewModel model,CreateJobPageResult data)
        {
            return new JobBrowseViewModel
            {
                SortOrder = model.SortOrder,
                CategoryId = model.CategoryId,
                SkillId = model.SkillId,
                MaxBudget = model.MaxBudget,
                PageNumber = model.PageNumber,
                PageSize = model.PageSize,
                TotalCount = result.TotalCount,
                Jobs = result.Jobs,
                Categories = data.Categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList(),
                Skills = data.Skills.Select(s => new SelectListItem(s.Name, s.Id.ToString())).ToList()
            };
        }

        private async Task PopulateCreateOptionsAsync(bool isCreate = false)
        {
            var result = await _jobService.GetCreateJobPageDataAsync();
            if (isCreate)
            {
                ViewBag.Categories = result.Categories.Select(c => new SelectableItem(c.Id.ToString(), c.Name)).ToList();
                ViewBag.Tags = result.Tags.Select(t => new SelectableItem(t.Id.ToString(), t.Name)).ToList();
                ViewBag.Skills = result.Skills.Select(s => new SelectableItem(s.Id.ToString(), s.Name)).ToList();
            }
            else
            {
                ViewBag.Categories = result.Categories;
                ViewBag.Tags = result.Tags;
                ViewBag.Skills = result.Skills;
            }
        }
    }
}
