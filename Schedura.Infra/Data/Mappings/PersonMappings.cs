using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Schedura.Domain.Entities;

namespace Schedura.Infra.Data.Mappings;

public class PersonMappings : IEntityTypeConfiguration<Person> {
	public void Configure(EntityTypeBuilder<Person> builder) {
		builder.ToTable("Persons");

		builder.HasKey(x => x.Id);

		builder.Property(x => x.Id)
			.HasMaxLength(32)
			.IsUnicode(false)
			.ValueGeneratedNever();

		builder.Property(x => x.Name)
			.HasMaxLength(120)
			.IsRequired();

		builder.Property(x => x.Surname)
			.HasMaxLength(120)
			.IsRequired();

		builder.Property(x => x.Email)
			.HasMaxLength(180)
			.IsRequired();

		builder.Property(x => x.PhoneNumber)
			.HasMaxLength(20)
			.IsRequired();

		builder.Property(x => x.Address)
			.HasMaxLength(250)
			.IsRequired();

		builder.Property(x => x.BirthDate)
			.IsRequired();

		builder.Property(x => x.Gender)
			.HasMaxLength(30)
			.IsRequired();

		builder.Property(x => x.Document)
			.HasMaxLength(18)
			.IsRequired();

		builder.Property(x => x.PersonType)
			.HasConversion<string>()
			.HasMaxLength(20)
			.IsRequired();

		builder.Property(x => x.CreatedAt)
			.IsRequired();

		builder.Property(x => x.UpdatedAt);

		builder.Property(x => x.DeletedAt);

		builder.Ignore(x => x.FormattedDocument);

		builder.HasIndex(x => x.Email)
			.IsUnique();

		builder.HasIndex(x => x.Document)
			.IsUnique();
	}
}
