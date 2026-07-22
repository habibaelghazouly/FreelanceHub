namespace FreelanceHub.Domain.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; } = null!;

		public ICollection<JobTag> JobTags { get; set; } = new List<JobTag>();
    }
}
