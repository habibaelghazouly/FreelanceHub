namespace FreelanceHub.Domain.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Job> Jobs { get; set; } = new List<Job>();

		public ICollection<JobCategory> JobCategories { get; set; } = new List<JobCategory>();
    }
}
