namespace FreelanceHub.Domain.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

		public ICollection<JobCategory> JobCategories { get; set; } = new List<JobCategory>();
    }
}
