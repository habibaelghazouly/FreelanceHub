namespace FreelanceHub.Domain.Models.Skills
{
	public class Skill
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();

		public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
	}
}