namespace FreelanceHub.Domain.Models
{
	public class FreelancerSkill
	{
		public int FreelancerProfileId { get; set; }

		// Requires the FreelancerProfile entity (not included in this delivery).
		// public FreelancerProfile FreelancerProfile { get; set; } = null!;

		public int SkillId { get; set; }

		public Skill Skill { get; set; } = null!;
	}
}