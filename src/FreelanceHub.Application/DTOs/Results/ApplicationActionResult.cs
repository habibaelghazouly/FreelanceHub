namespace FreelanceHub.Application.DTOs.Results
{
    public class ApplicationActionResult
    {
        private ApplicationActionResult(bool succeeded, IReadOnlyList<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors;
        }

        public bool Succeeded { get; }

        public IReadOnlyList<string> Errors { get; }

        public static ApplicationActionResult Success()
        {
            return new ApplicationActionResult(true, Array.Empty<string>());
        }
        public static ApplicationActionResult Success(int jobId)
        {
            return new ApplicationActionResult(true, new[] { jobId.ToString() });
        }
        public static ApplicationActionResult Failed(params string[] errors)
        {
            return new ApplicationActionResult(false, errors);
        }

        public static ApplicationActionResult Failed(IEnumerable<string> errors)
        {
            return new ApplicationActionResult(false, errors.ToArray());
        }
    }
}
