using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedura.Domain.Entities;

namespace Schedura.Infra.Data.Mappings;

public class RefreshTokenMappings : IEntityTypeConfiguration<RefreshToken> {
	public void Configure(EntityTypeBuilder<RefreshToken> builder) {
		builder.ToTable("RefreshTokens");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasMaxLength(32)
			.IsUnicode(false)
			.ValueGeneratedNever();

		builder.Property(x => x.Token)
			.HasMaxLength(255)
			.IsRequired();

		builder.Property(x => x.UserId)
			.HasMaxLength(32)
			.IsUnicode(false)
			.IsRequired();

		builder.Property(x => x.ExpiresAt)
			.IsRequired();

		builder.Property(x => x.RevokedAt);

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.Property(x => x.DeviceInfo)
			.HasMaxLength(255);

		builder.HasIndex(x => x.Token)
			.IsUnique();

		builder.HasOne(x => x.User)
			.WithMany(u => u.RefreshTokens)
			.HasForeignKey(x => x.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
