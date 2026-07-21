namespace FreelanceHub.Web.ViewModels
{
    public class CreateJobViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Budget { get; set; }
        public DateTime Deadline { get; set; }

        public string CategoryIds { get; set; } = string.Empty;
        public string SkillIds { get; set; } = string.Empty;
        public string TagIds { get; set; } = string.Empty;
    }
}