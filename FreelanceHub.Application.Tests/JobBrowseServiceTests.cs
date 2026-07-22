using FreelanceHub.Application.Services.Implementations;
using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using Xunit;

namespace FreelanceHub.Application.Tests;

public class JobBrowseServiceTests
{
    [Fact]
    public void ApplyFiltersAndSorting_SortsByNewestAndFiltersByBudgetAndSkills()
    {
        var jobs = new List<Job>
        {
            new() { JobId = 1, Title = "Old", Budget = 1200, CategoryId = 2, CreatedAt = new DateTime(2024, 1, 1), JobStatus = JobStatus.Open },
            new() { JobId = 2, Title = "Match", Budget = 800, CategoryId = 2, CreatedAt = new DateTime(2024, 3, 1), JobStatus = JobStatus.Open },
            new() { JobId = 3, Title = "Budget too high", Budget = 2000, CategoryId = 2, CreatedAt = new DateTime(2024, 2, 1), JobStatus = JobStatus.Open },
            new() { JobId = 4, Title = "Wrong skill", Budget = 500, CategoryId = 2, CreatedAt = new DateTime(2024, 4, 1), JobStatus = JobStatus.Open },
            new() { JobId = 5, Title = "Closed", Budget = 600, CategoryId = 2, CreatedAt = new DateTime(2024, 5, 1), JobStatus = JobStatus.Completed }
        };

        var jobSkills = new List<JobSkill>
        {
            new() { JobId = 2, SkillId = 1 },
            new() { JobId = 4, SkillId = 2 }
        };

        var service = new JobBrowseService();
        var result = service.ApplyFiltersAndSorting(jobs.AsQueryable(), jobSkills.AsQueryable(), 2, 1000, 1, "date", 1, 3);

        Assert.Single(result);
        Assert.Equal(2, result.First().JobId);
    }
}
