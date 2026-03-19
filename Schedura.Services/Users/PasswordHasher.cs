using System.Security.Cryptography;

namespace Schedura.Services.Users;

public static class PasswordHasher {
	private const int SaltSize = 16;
	private const int KeySize = 32;
	private const int Iterations = 100_000;

	public static string Hash(string password) {
		if (string.IsNullOrWhiteSpace(password)) {
			throw new ArgumentException("Password não pode ser vazio.", nameof(password));
		}

		Span<byte> salt = stackalloc byte[SaltSize];
		RandomNumberGenerator.Fill(salt);

		var key = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
	}
}
