using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedura.Domain.Entities;

namespace Schedura.Infra.Data.Mappings;

public class UserGroupPermissionMappings : IEntityTypeConfiguration<UserGroupPermission> {
	public void Configure(EntityTypeBuilder<UserGroupPermission> builder) {
		builder.ToTable("UserGroupPermissions");

		builder.HasKey(x => new { x.UserGroupId, x.PermissionId });

		builder.Property(x => x.UserGroupId)
			.HasMaxLength(32)
			.IsUnicode(false)
			.IsRequired();

		builder.Property(x => x.PermissionId)
			.HasMaxLength(32)
			.IsUnicode(false)
			.IsRequired();

		builder.HasOne(x => x.Group)
			.WithMany(g => g.Permissions)
			.HasForeignKey(x => x.UserGroupId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(x => x.Permission)
			.WithMany(p => p.UserGroupPermissions)
			.HasForeignKey(x => x.PermissionId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
