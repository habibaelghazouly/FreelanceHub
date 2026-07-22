namespace FreelanceHub.Application.DTOs.Results
{
    public class CreateJobResult
    {
        public bool Succeeded { get; set; }
        public int JobId { get; set; } = 0;
    }
}