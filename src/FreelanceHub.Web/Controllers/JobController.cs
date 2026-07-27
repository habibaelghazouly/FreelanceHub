using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.DTOs.Results;
using FreelanceHub.Application.Services.Abstractions;
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
            var result = await _jobService.GetCreateJobPageDataAsync();

            ViewBag.Categories = result.Categories;
            ViewBag.Tags = result.Tags;
            ViewBag.Skills = result.Skills;

            return View(new CreateJobViewModel());
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

            try
            {
                foreach (var jobFile in model.JobFiles.Where(file => file.Length > 0))
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
    }
}