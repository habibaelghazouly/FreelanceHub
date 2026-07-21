namespace FreelanceHub.Application.DTOs.Results
{
	public class UpdateOperationResult
	{
		private UpdateOperationResult(bool succeeded, bool notFound, IReadOnlyList<UpdateOperationError> errors)
		{
			Succeeded = succeeded;
			NotFound = notFound;
			Errors = errors;
		}

		public bool Succeeded { get; }

		public bool NotFound { get; }

		public IReadOnlyList<UpdateOperationError> Errors { get; }

		public static UpdateOperationResult Success()
		{
			return new UpdateOperationResult(true, false, Array.Empty<UpdateOperationError>());
		}

		public static UpdateOperationResult Missing()
		{
			return new UpdateOperationResult(false, true, Array.Empty<UpdateOperationError>());
		}

		public static UpdateOperationResult Failed(params UpdateOperationError[] errors)
		{
			return new UpdateOperationResult(false, false, errors);
		}
	}

	public class UpdateOperationError
	{
		public UpdateOperationError(string? fieldName, string message)
		{
			FieldName = fieldName;
			Message = message;
		}

		public string? FieldName { get; }

		public string Message { get; }
	}

	public class AccountDetailsResult
	{
		public string Username { get; set; } = string.Empty;

		public string FirstName { get; set; } = string.Empty;

		public string LastName { get; set; } = string.Empty;

		public string Email { get; set; } = string.Empty;

		public bool IsEmailConfirmed { get; set; }
	}
}
