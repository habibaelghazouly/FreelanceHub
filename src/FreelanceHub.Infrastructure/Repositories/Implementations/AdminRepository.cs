using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations;

public class AdminRepository(ApplicationDbContext dbContext) : IAdminRepository
{
    public async Task<IReadOnlyList<ApplicationUser>> ListUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default) =>
        await (from userRole in dbContext.UserRoles
               join role in dbContext.Roles on userRole.RoleId equals role.Id
               join user in dbContext.Users on userRole.UserId equals user.Id
               where role.Name == roleName
               orderby user.FirstName, user.LastName
               select user).AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Job>> ListJobsAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Jobs.IgnoreQueryFilters().AsNoTracking()
            .Include(job => job.ClientUser)
            .Include(job => job.Applications)
            .Include(job => job.Contract)
            .OrderByDescending(job => job.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Contract>> ListContractsAsync(CancellationToken cancellationToken = default) =>
        // Administrators audit every established contract, including contracts whose job was later revoked.
        await dbContext.Contracts.IgnoreQueryFilters().AsNoTracking()
            .Include(contract => contract.Job).ThenInclude(job => job.ClientUser)
            .Include(contract => contract.AcceptedApplication).ThenInclude(application => application.FreelancerUser)
            .OrderByDescending(contract => contract.StartDate)
            .ToListAsync(cancellationToken);

    public Task<Job?> GetJobForRevocationAsync(int jobId, CancellationToken cancellationToken = default) =>
        dbContext.Jobs.Include(job => job.Applications).Include(job => job.Contract)
            .SingleOrDefaultAsync(job => job.JobId == jobId, cancellationToken);
}
