using Schedura.Domain.Enums;

namespace Schedura.Domain.Entities;

public class Person(string name, string surname, string email, string phoneNumber, string address, DateOnly birthDate, string gender, string document, PersonType personType) : BaseEntity {
	public string Name { get; private set; } = name;
	public string Surname { get; private set; } = surname;
	public string Email { get; private set; } = email;
	public string PhoneNumber { get; private set; } = phoneNumber;
	public string Address { get; private set; } = address;
	public DateOnly BirthDate { get; private set; } = birthDate;
	public string Gender { get; private set; } = gender;
	public string Document { get; private set; } = document;
	public PersonType PersonType { get; private set; } = personType;
	public string FormattedDocument => FormatDocument(Document, PersonType);

	public void SetEmail(string email) {
		Email = email;
		SetUpdatedAt();
	}

	public void SetPhoneNumber(string phoneNumber) {
		PhoneNumber = phoneNumber;
		SetUpdatedAt();
	}

	public void SetAddress(string address) {
		Address = address;
		SetUpdatedAt();
	}

	public void SetBirthDate(DateOnly birthDate) {
		BirthDate = birthDate;
		SetUpdatedAt();
	}

	public void SetGender(string gender) {
		Gender = gender;
		SetUpdatedAt();
	}

	public void SetDocument(string document) {
		Document = document;
		SetUpdatedAt();
	}

	private static string FormatDocument(string document, PersonType personType) {
		var digitsOnly = new string(document.Where(char.IsDigit).ToArray());

		return personType switch {
			PersonType.Individual when digitsOnly.Length == 11 =>
				$"{digitsOnly[..3]}.{digitsOnly[3..6]}.{digitsOnly[6..9]}-{digitsOnly[9..]}",
			PersonType.Company when digitsOnly.Length == 14 =>
				$"{digitsOnly[..2]}.{digitsOnly[2..5]}.{digitsOnly[5..8]}/{digitsOnly[8..12]}-{digitsOnly[12..]}",
			_ => document
		};
	}
}
