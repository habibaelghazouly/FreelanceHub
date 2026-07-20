using FreelanceHub.Application.Services.Abstractions;
using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Web.Controllers;

[Authorize(Roles = "Freelancer")]
public class JobsController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IJobBrowseService _jobBrowseService;

    public JobsController(ApplicationDbContext dbContext, IJobBrowseService jobBrowseService)
    {
        _dbContext = dbContext;
        _jobBrowseService = jobBrowseService;
    }

    [HttpGet]
    public async Task<IActionResult> Browse(JobBrowseViewModel model)
    {
        var jobsQuery = _dbContext.Jobs.AsNoTracking().Where(job => !job.IsDeleted);
        var jobSkillsQuery = _dbContext.JobSkills.AsNoTracking();

        var filteredJobs = _jobBrowseService.ApplyFiltersAndSorting(
            jobsQuery,
            jobSkillsQuery,
            model.CategoryId,
            model.MaxBudget,
            model.SkillId,
            model.SortOrder,
            model.PageNumber,
            model.PageSize);

        var totalCountQuery = _jobBrowseService.ApplyFiltersAndSorting(
            jobsQuery,
            jobSkillsQuery,
            model.CategoryId,
            model.MaxBudget,
            model.SkillId,
            model.SortOrder,
            1,
            int.MaxValue);

        var totalCount = await totalCountQuery.CountAsync();
        var jobs = await filteredJobs.ToListAsync();

        var viewModel = new JobBrowseViewModel
        {
            SortOrder = model.SortOrder,
            CategoryId = model.CategoryId,
            SkillId = model.SkillId,
            MaxBudget = model.MaxBudget,
            PageNumber = model.PageNumber,
            PageSize = model.PageSize,
            TotalCount = totalCount,
            Jobs = jobs,
            Categories = await _dbContext.Categories.AsNoTracking()
                .Select(category => new SelectListItem(category.Name, category.Id.ToString()))
                .ToListAsync(),
            Skills = await _dbContext.Skills.AsNoTracking()
                .Select(skill => new SelectListItem(skill.Name, skill.Id.ToString()))
                .ToListAsync()
        };

        return View(viewModel);
    }
}
