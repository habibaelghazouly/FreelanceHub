using FreelanceHub.Domain.Models;
using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
    public class LookupRepository : ILookupRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LookupRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Categories.ToListAsync(cancellationToken);
        }

        public async Task<List<Tag>> GetTagsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Tags.ToListAsync(cancellationToken);
        }

        public async Task<List<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Skills.ToListAsync(cancellationToken);
        }
    }
}