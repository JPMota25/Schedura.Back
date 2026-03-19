using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedura.Domain.Entities;

namespace Schedura.Infra.Data.Mappings;

public class UserUserGroupMappings : IEntityTypeConfiguration<UserUserGroup> {
	public void Configure(EntityTypeBuilder<UserUserGroup> builder) {
		builder.ToTable("UserUserGroups");

		builder.HasKey(x => new { x.UserId, x.UserGroupId });

		builder.Property(x => x.UserId)
			.HasMaxLength(32)
			.IsUnicode(false)
			.IsRequired();

		builder.Property(x => x.UserGroupId)
			.HasMaxLength(32)
			.IsUnicode(false)
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.HasOne(x => x.User)
			.WithMany(u => u.Groups)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.Group)
			.WithMany(g => g.UserGroups)
			.HasForeignKey(x => x.UserGroupId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
