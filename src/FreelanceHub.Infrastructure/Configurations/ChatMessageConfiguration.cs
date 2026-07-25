using FreelanceHub.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FreelanceHub.Infrastructure.Configurations
{
	public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
	{
		public void Configure(EntityTypeBuilder<ChatMessage> builder)
		{
			builder.ToTable("chat_messages");
			builder.HasKey(message => message.ChatMessageId);

			builder.Property(message => message.ChatMessageId).HasColumnName("chat_message_id");
			builder.Property(message => message.ApplicationId).HasColumnName("application_id").IsRequired();
			builder.Property(message => message.SenderUserId).HasColumnName("sender_user_id").IsRequired();
			builder.Property(message => message.Content).HasColumnName("content").HasMaxLength(2000).IsRequired();
			builder.Property(message => message.SentAt).HasColumnName("sent_at").HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();

			builder.HasIndex(message => new { message.ApplicationId, message.SentAt, message.ChatMessageId });
			builder.HasIndex(message => message.SenderUserId);
			builder.HasQueryFilter(message => !message.Application.Job.IsDeleted);

			builder.HasOne(message => message.Application)
				.WithMany(application => application.ChatMessages)
				.HasForeignKey(message => message.ApplicationId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(message => message.SenderUser)
				.WithMany(user => user.SentChatMessages)
				.HasForeignKey(message => message.SenderUserId)
				.OnDelete(DeleteBehavior.NoAction);
		}
	}
}
