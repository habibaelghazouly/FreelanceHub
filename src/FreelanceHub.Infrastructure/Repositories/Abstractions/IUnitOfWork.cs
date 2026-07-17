namespace FreelanceHub.Infrastructure.Repositories.Abstractions
{
	public interface IUnitOfWork : IAsyncDisposable
	{
		Task BeginTransactionAsync(CancellationToken cancellationToken = default);

		Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

		Task CommitTransactionAsync(CancellationToken cancellationToken = default);

		Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
	}
}
