using Microsoft.AspNetCore.Identity;

namespace FreelanceHub.Application.DTOs.Results
{
	public class ApplicationUserServiceResult
	{
		private ApplicationUserServiceResult(bool succeeded, IReadOnlyList<string> errors, bool isLockedOut = false)
		{
			Succeeded = succeeded;
			Errors = errors;
			IsLockedOut = isLockedOut;
		}

		public bool Succeeded { get; }

		public IReadOnlyList<string> Errors { get; }

		public bool IsLockedOut { get; }

		public static ApplicationUserServiceResult Success()
		{
			return new ApplicationUserServiceResult(true, Array.Empty<string>());
		}

		public static ApplicationUserServiceResult Failed(params string[] errors)
		{
			return new ApplicationUserServiceResult(false, errors);
		}

		public static ApplicationUserServiceResult Failed(IEnumerable<IdentityError> errors)
		{
			return new ApplicationUserServiceResult(false, errors.Select(error => error.Description).ToArray());
		}

		public static ApplicationUserServiceResult LockedOut(string error)
		{
			return new ApplicationUserServiceResult(false, new[] { error }, true);
		}
	}
}
