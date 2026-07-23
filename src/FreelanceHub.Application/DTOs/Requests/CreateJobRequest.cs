using Microsoft.AspNetCore.Http;

namespace FreelanceHub.Application.DTOs.Requests
{
    public class CreateJobRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public DateTime Deadline { get; set; }
        public int ClientId { get; set; }

        public string CategoryIds { get; set; } = string.Empty;

        public string SkillIds { get; set; } = string.Empty;

        public string TagIds { get; set; } = string.Empty;

        public IReadOnlyList<UploadedFileRequest> JobFiles { get; set; } = Array.Empty<UploadedFileRequest>();
    }
}