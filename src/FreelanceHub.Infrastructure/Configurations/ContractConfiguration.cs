using FreelanceHub.Domain.Enums;
using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
    public class ContractConfiguration : IEntityTypeConfiguration<Contract>
    {
        public void Configure(EntityTypeBuilder<Contract> builder)
        {
            builder.ToTable("contracts", table =>
            {
                table.HasCheckConstraint("chk_contracts_agreed_amount", "[agreed_amount] >= 0");
                table.HasCheckConstraint("chk_contracts_status", "[contract_status] IN (60, 61, 62, 63, 64,65)");
                table.HasCheckConstraint("chk_contracts_expected_completion", "[expected_completion_date] IS NULL OR [expected_completion_date] >= [start_date]");
                table.HasCheckConstraint("chk_contracts_actual_completion", "[actual_completion_date] IS NULL OR [actual_completion_date] >= [start_date]");
            });

            builder.HasKey(contract => contract.ContractId);

            builder.Property(contract => contract.ContractId).HasColumnName("contract_id");
            builder.Property(contract => contract.JobId).HasColumnName("job_id").IsRequired();
            builder.Property(contract => contract.AcceptedApplicationId).HasColumnName("accepted_application_id").IsRequired();
            builder.Property(contract => contract.AgreedAmount).HasColumnName("agreed_amount").HasPrecision(18, 2).IsRequired();
            builder.Property(contract => contract.ContractStatus).HasColumnName("contract_status").HasConversion<int>().HasDefaultValue(ContractStatus.Draft).HasSentinel((ContractStatus)0).IsRequired();
            builder.Property(contract => contract.StartDate).HasColumnName("start_date").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(contract => contract.ExpectedCompletionDate).HasColumnName("expected_completion_date");
            builder.Property(contract => contract.ActualCompletionDate).HasColumnName("actual_completion_date");
            builder.Property(contract => contract.CreatedAt).HasColumnName("created_at").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");
            builder.Property(contract => contract.UpdatedAt).HasColumnName("updated_at").IsRequired().HasDefaultValueSql("SYSUTCDATETIME()");

            builder.HasQueryFilter(contract => !contract.Job.IsDeleted);
        
            builder.HasOne(contract => contract.Job)
                .WithOne(job => job.Contract)
                .HasForeignKey<Contract>(contract => contract.JobId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(contract => contract.AcceptedApplication)
               .WithOne(application => application.Contract)
               .HasForeignKey<Contract>(contract => new { contract.AcceptedApplicationId, contract.JobId })
               .HasPrincipalKey<Application>(application => new { application.ApplicationId, application.JobId })
               .OnDelete(DeleteBehavior.NoAction);

            //builder.HasMany(contract => contract.Reviews)
            //    .WithOne(review => review.Contract)
            //    .HasForeignKey(review => review.ContractId)
            //    .OnDelete(DeleteBehavior.NoAction);

            //builder.HasMany(contract => contract.ContractAttachments)
            //    .WithOne(ca => ca.Contract)
            //    .HasForeignKey(ca => ca.ContractId)
            //    .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
