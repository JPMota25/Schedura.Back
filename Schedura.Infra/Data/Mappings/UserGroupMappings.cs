using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedura.Domain.Entities;

namespace Schedura.Infra.Data.Mappings;

public class UserGroupMappings : IEntityTypeConfiguration<UserGroup> {
	public void Configure(EntityTypeBuilder<UserGroup> builder) {
		builder.ToTable("UserGroups");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasMaxLength(32)
			.IsUnicode(false)
			.ValueGeneratedNever();

		builder.Property(x => x.Name)
			.HasMaxLength(80)
			.IsRequired();

		builder.Property(x => x.Description)
			.HasMaxLength(255);

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.Property(x => x.UpdatedAt);
		builder.Property(x => x.DeletedAt);

		builder.HasIndex(x => x.Name)
			.IsUnique();
	}
}
