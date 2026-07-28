namespace FreelanceHub.Application.DTOs.Results
{
    public class CreateJobResult
    {
        public bool Succeeded { get; set; }
        public int JobId { get; set; } = 0;
        public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();
    }
}
