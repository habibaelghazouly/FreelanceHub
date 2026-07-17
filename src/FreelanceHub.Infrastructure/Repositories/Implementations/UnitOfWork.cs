using FreelanceHub.Infrastructure.DataBase;
using FreelanceHub.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore.Storage;

namespace FreelanceHub.Infrastructure.Repositories.Implementations
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly ApplicationDbContext _dbContext;
		private IDbContextTransaction? _transaction;

		public UnitOfWork(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction is not null)
			{
				throw new InvalidOperationException("A database transaction is already active.");
			}

			_transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
		}

		public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return _dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction is null)
			{
				throw new InvalidOperationException("No database transaction is active.");
			}

			try
			{
				await _transaction.CommitAsync(cancellationToken);
			}
			finally
			{
				await DisposeTransactionAsync();
			}
		}

		public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
		{
			if (_transaction is null)
			{
				return;
			}

			try
			{
				await _transaction.RollbackAsync(cancellationToken);
			}
			finally
			{
				await DisposeTransactionAsync();
			}
		}

		public ValueTask DisposeAsync()
		{
			return DisposeTransactionAsync();
		}

		private async ValueTask DisposeTransactionAsync()
		{
			if (_transaction is null)
			{
				return;
			}

			await _transaction.DisposeAsync();
			_transaction = null;
		}
	}
}
