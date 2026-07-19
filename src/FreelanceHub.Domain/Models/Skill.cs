namespace FreelanceHub.Domain.Models
{
	public class Skill
	{
		public int SkillId { get; set; }

		public string Name { get; set; } = string.Empty;

		public ICollection<FreelancerSkill> FreelancerSkills { get; set; } = new List<FreelancerSkill>();

		public ICollection<JobSkill> JobSkills { get; set; } = new List<JobSkill>();
	}
}
