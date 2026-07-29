using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
   public interface ILookupRepository
{
    Task<List<Category>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<List<Tag>> GetTagsAsync(CancellationToken cancellationToken = default);
    Task<List<Skill>> GetSkillsAsync(CancellationToken cancellationToken = default);
}
}