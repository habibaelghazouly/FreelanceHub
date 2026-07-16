using FreelanceHub.Domain.Models;

namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IClientProfileRepository
	{
		Task AddAsync(ClientProfile clientProfile, CancellationToken cancellationToken = default);
	}
}
