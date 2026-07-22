using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class ReviewConfiguration : IEntityTypeConfiguration<Review>
	{
		public void Configure(EntityTypeBuilder<Review> builder)
		{
			builder.ToTable("reviews", table =>
			{
				table.HasCheckConstraint("chk_reviews_rating", "[rating] BETWEEN 1 AND 5");
				table.HasCheckConstraint("chk_reviews_users", "[reviewer_user_id] <> [reviewee_user_id]");
			});

			builder.HasKey(review => review.ReviewId);

			builder.Property(review => review.ReviewId).HasColumnName("review_id");
			builder.Property(review => review.ContractId).HasColumnName("contract_id").IsRequired();
			builder.Property(review => review.ReviewerUserId).HasColumnName("reviewer_user_id").IsRequired();
			builder.Property(review => review.RevieweeUserId).HasColumnName("reviewee_user_id").IsRequired();
			builder.Property(review => review.Rating).HasColumnName("rating").IsRequired();
			builder.Property(review => review.Comment).HasColumnName("comment").HasMaxLength(1000);
			builder.Property(review => review.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();

			builder.HasIndex(review => new { review.ContractId, review.ReviewerUserId }).IsUnique();
			builder.HasIndex(review => review.RevieweeUserId);
			builder.HasQueryFilter(review => !review.Contract.Job.IsDeleted);

			builder.HasOne(review => review.Contract)
				.WithMany(contract => contract.Reviews)
				.HasForeignKey(review => review.ContractId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(review => review.ReviewerUser)
				.WithMany()
				.HasForeignKey(review => review.ReviewerUserId)
				.OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(review => review.RevieweeUser)
				.WithMany()
				.HasForeignKey(review => review.RevieweeUserId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
