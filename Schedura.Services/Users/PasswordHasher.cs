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

	public static bool Verify(string password, string hash) {
		if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash)) {
			return false;
		}

		var parts = hash.Split('$');
		if (parts.Length != 4 || parts[0] != "PBKDF2") {
			return false;
		}

		if (!int.TryParse(parts[1], out var iterations)) return false;

		byte[] salt;
		byte[] expectedKey;
		try {
			salt = Convert.FromBase64String(parts[2]);
			expectedKey = Convert.FromBase64String(parts[3]);
		}
		catch {
			return false;
		}

		var actualKey = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			iterations,
			HashAlgorithmName.SHA256,
			expectedKey.Length);

		return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
	}
}
