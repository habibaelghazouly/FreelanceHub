using FreelanceHub.Application.DTOs.Requests;
using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FreelanceHub.Web.Controllers
{
    [Authorize]
    public class ApplicationsController : Controller
    {
        private readonly IApplicationManagementService _applicationManagementService;
        private readonly IApplicationRepository _applicationRepository;

        public ApplicationsController(IApplicationManagementService applicationManagementService, IApplicationRepository applicationRepository  )
        {
            _applicationManagementService = applicationManagementService;
            _applicationRepository = applicationRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> Submit(int jobId)
        {

            if (jobId <= 0)
            {
                return BadRequest("A valid Job ID is required to apply.");
            }
            if (!TryGetCurrentUserId(out var freelancerUserId))
            {
                return Forbid();
            }

            var job = await _applicationManagementService.GetOpenJobByIdAsync(jobId, HttpContext.RequestAborted);
            if (job is null)
            {
                return NotFound("The requested job is not available for application.");
            }
            var alreadyApplied = await _applicationRepository.HasFreelancerAppliedAsync(jobId, freelancerUserId, HttpContext.RequestAborted);

            if (alreadyApplied)
            {
                
                ViewBag.JobTitle = job.Title;
                return View("AlreadySubmitted");
            }

            return View(new SubmitApplicationViewModel
            {
                JobId = job.JobId,
                JobTitle = job.Title
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
                return RedirectToAction(nameof(MyApplications));
            }
            finally
            {
                foreach (var stream in openedStreams)
                {
                    stream.Dispose();
                }
            }
        }

        [HttpGet]
        [Authorize(Roles = "Freelancer")]
        public async Task<IActionResult> MyApplications()
        {
            if (!TryGetCurrentUserId(out var freelancerUserId))
            {
                return Forbid();
            }

            var dashboard = await _applicationManagementService.GetFreelancerDashboardAsync(freelancerUserId, HttpContext.RequestAborted);

            return View(new FreelancerApplicationDashboardViewModel
            {
                Applications = dashboard.Applications.Select(item => new FreelancerApplicationItemViewModel
                {
                    ApplicationId = item.ApplicationId,
                    JobId = item.JobId,
                    JobTitle = item.JobTitle,
                    ProposedAmount = item.ProposedAmount,
                    TimelineDays = item.TimelineDays,
                    ApplicationStatus = item.ApplicationStatus,
                    PortfolioItemCount = item.PortfolioItemCount,
                    SubmittedAt = item.CreatedAt
                }).ToArray()
            });
        }

        [HttpGet]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> SubmittedApplications(int jobId)
        {
            if (jobId <= 0)
            {
                return BadRequest("A valid Job ID is required.");
            }

            if (!TryGetCurrentUserId(out var clientUserId))
            {
                return Forbid();
            }

            var job = await _applicationManagementService.GetOpenJobByIdAsync(jobId, HttpContext.RequestAborted);
            if (job == null)
            {
                return NotFound("Job not found.");
            }

            var applications = await _applicationManagementService.GetApplicationsForJobAsync(jobId, clientUserId, HttpContext.RequestAborted);

            var viewModel = new SubmittedApplicationsListViewModel
            {
                JobId = job.JobId,
                JobTitle = job.Title,
                Applications = applications.Select(a => new SubmittedApplicationViewModel
                {
                    ApplicationId = a.ApplicationId,
                    JobId = a.JobId,
                    FreelancerUserId = a.FreelancerUserId,
                    FreelancerName = a.FreelancerUser.UserName,
                    ProposedAmount = a.ProposedAmount,
                    TimelineDays = a.TimelineDays,
                    CoverLetter = a.CoverLetter,
                    SubmittedAt = a.CreatedAt,
                    ApplicationStatus = a.ApplicationStatus
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Client")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int applicationId, int jobId, ApplicationStatus applicationStatus)
        {
            if (!TryGetCurrentUserId(out var clientUserId))
            {
                return Forbid();
            }

            var result = await _applicationManagementService.UpdateApplicationStatusAsync(new UpdateApplicationStatusRequest
            {
                ApplicationId = applicationId,
                ClientUserId = clientUserId,
                ApplicationStatus = applicationStatus
            }, HttpContext.RequestAborted);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Application status updated.";
            }
            else
            {
                TempData["ErrorMessage"] = string.Join(" ", result.Errors);
            }

            return RedirectToAction(nameof(SubmittedApplications), new { jobId = jobId });
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claimValue, out userId);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            if (!TryGetCurrentUserId(out var currentUserId))
            {
                return Forbid();
            }

            try
            {
                var application = await _applicationManagementService.GetApplicationByIdAsync(id, currentUserId, HttpContext.RequestAborted);
                if (application == null)
                {
                    return NotFound();
                }

                var isClient = User.IsInRole("Client");

                var viewModel = new ApplicationDetailsViewModel
                {
                    ApplicationId = application.ApplicationId,
                    JobId = application.JobId,
                    JobTitle = application.Job?.Title,
                    FreelancerUserId = application.FreelancerUserId,
                    FreelancerName = application.FreelancerUser?.UserName ?? "Freelancer",
                    ProposedAmount = application.ProposedAmount,
                    TimelineDays = application.TimelineDays,
                    CoverLetter = application.CoverLetter,
                    SubmittedAt = application.CreatedAt,
                    ApplicationStatus = application.ApplicationStatus,
                    IsClient = isClient,

                    Attachments = application.ApplicationAttachments?.Select(item =>
                    {
                        var fileName = item.Attachment?.OriginalFileName ?? "Attachment";

                        // Make sure we access the full relative path or append the stored file name
                        var rawPath = item.Attachment?.FileUrl
                                   ?? $"/uploads/application-portfolios/{item.Attachment?.StoredFileName}";

                        // Ensure leading slash
                        if (!string.IsNullOrEmpty(rawPath) && !rawPath.StartsWith("/"))
                        {
                            rawPath = "/" + rawPath;
                        }

                        return new ApplicationAttachmentViewModel
                        {
                            AttachmentId = item.AttachmentId,
                            FileName = fileName,
                            FilePath = rawPath,
                            FileExtension = item.Attachment?.ContentType
                                             ?? Path.GetExtension(fileName)
                        };
                    }).ToList() ?? new List<ApplicationAttachmentViewModel>()
                };

                return View(viewModel);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
    }
 

