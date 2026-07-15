using FreelanceHub.Domain.Models.Attachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations.Attachments
{
	public class ContractAttachmentConfiguration : IEntityTypeConfiguration<ContractAttachment>
	{
		public void Configure(EntityTypeBuilder<ContractAttachment> builder)
		{
			builder.ToTable("contract_attachments");

			builder.HasKey(ca => new { ca.ContractId, ca.AttachmentId });

			builder.Property(ca => ca.ContractId).HasColumnName("contract_id");
			builder.Property(ca => ca.AttachmentId).HasColumnName("attachment_id");

			// Requires the Contract entity (not included in this delivery).
			// builder.HasOne(ca => ca.Contract)
			// 	.WithMany(contract => contract.ContractAttachments)
			// 	.HasForeignKey(ca => ca.ContractId)
			// 	.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(ca => ca.Attachment)
				.WithMany(attachment => attachment.ContractAttachments)
				.HasForeignKey(ca => ca.AttachmentId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}
