using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedura.Domain.Entities;

namespace Schedura.Infra.Data.Mappings;

public class UserMappings : IEntityTypeConfiguration<User> {
	public void Configure(EntityTypeBuilder<User> builder) {
		builder.ToTable("Users");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasMaxLength(32)
			.IsUnicode(false)
			.ValueGeneratedNever();

		builder.Property(x => x.Username)
			.HasMaxLength(120)
			.IsRequired();

		builder.Property(x => x.Password)
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(x => x.PersonId)
			.HasMaxLength(32)
			.IsUnicode(false)
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.Property(x => x.UpdatedAt);

		builder.Property(x => x.DeletedAt);

		builder.HasIndex(x => x.Username)
			.IsUnique();

		builder.HasOne(x => x.Person)
			.WithOne()
			.HasForeignKey<User>(x => x.PersonId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}
