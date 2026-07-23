using FreelanceHub.Domain.Enums;

namespace FreelanceHub.Application.DTOs.Requests
{
    public class UpdateApplicationStatusRequest
    {
        public int ApplicationId { get; set; }

        public int ClientUserId { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
