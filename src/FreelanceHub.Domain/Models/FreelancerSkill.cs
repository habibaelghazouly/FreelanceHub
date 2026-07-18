namespace FreelanceHub.Domain.Models
{
	public class FreelancerSkill
	{
		public int FreelancerProfileId { get; set; }

		public FreelancerProfile FreelancerProfile { get; set; } = null!;

		public int SkillId { get; set; }

		public Skill Skill { get; set; } = null!;
	}
}
