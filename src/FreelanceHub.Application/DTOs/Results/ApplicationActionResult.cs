namespace FreelanceHub.Application.DTOs.Results
{
    public class ApplicationActionResult
    {
        private ApplicationActionResult(bool succeeded, IReadOnlyList<string> errors, int? jobId = null)
        {
            Succeeded = succeeded;
            Errors = errors;
            JobId = jobId;
        }

        public bool Succeeded { get; }

        public IReadOnlyList<string> Errors { get; }

        public int? JobId { get; }

        public static ApplicationActionResult Success()
        {
            return new ApplicationActionResult(true, Array.Empty<string>());
        }
        public static ApplicationActionResult Success(int jobId)
        {
            return new ApplicationActionResult(true, Array.Empty<string>(), jobId);
        }

        public static ApplicationActionResult Failed(int jobId, params string[] errors)
        {
            return new ApplicationActionResult(false, errors, jobId);
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
