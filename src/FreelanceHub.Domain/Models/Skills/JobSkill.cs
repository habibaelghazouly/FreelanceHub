namespace FreelanceHub.Domain.Models.Skills
{
	public class JobSkill
	{
		public int JobId { get; set; }

		// Requires the Job entity (not included in this delivery).
		// public Job Job { get; set; } = null!;

		public int SkillId { get; set; }

		public Skill Skill { get; set; } = null!;
	}
}