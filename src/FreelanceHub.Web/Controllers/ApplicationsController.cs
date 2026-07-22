using System.Security.Claims;
using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreelanceHub.Web.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationManagementService _applicationManagementService;

        public ApplicationsController(IApplicationManagementService applicationManagementService)
        {
            _applicationManagementService = applicationManagementService;
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public IActionResult Submit(int? jobId = null)
        {
            return View(new SubmitApplicationViewModel
            {
                JobId = jobId ?? 0
            });
        }

        [HttpPost]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Submit(SubmitApplicationViewModel model)
        {
            if (!TryGetCurrentUserId(out var freelancerUserId))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var openedStreams = new List<Stream>();
            try
            {
                var portfolioFiles = new List<UploadedFileRequest>();
                foreach (var portfolioFile in model.PortfolioFiles.Where(file => file.Length > 0))
                {
                    var stream = portfolioFile.OpenReadStream();
                    openedStreams.Add(stream);
                    portfolioFiles.Add(new UploadedFileRequest(stream, portfolioFile.FileName, portfolioFile.ContentType, portfolioFile.Length));
                }

                var result = await _applicationManagementService.SubmitApplicationAsync(new SubmitApplicationRequest
                {
                    JobId = model.JobId,
                    FreelancerUserId = freelancerUserId,
                    ProposedAmount = model.ProposedAmount!.Value,
                    CoverLetter = model.CoverLetter,
                    TimelineDays = model.TimelineDays!.Value,
                    PortfolioFiles = portfolioFiles
                }, HttpContext.RequestAborted);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }

                    return View(model);
                }

                TempData["SuccessMessage"] = "Application submitted successfully.";
                return RedirectToAction("Index" , "Home");
            }
            finally
            {
                foreach (var stream in openedStreams)
                {
                    stream.Dispose();
                }
            }
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }
    }
}
