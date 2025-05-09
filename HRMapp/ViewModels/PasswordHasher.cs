using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HRMapp.ViewModels
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16; // 128 bit
        private const int KeySize = 32;  // 256 bit
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;

        public static string Hash(string password)
        {
            // Generate a random salt
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            // Derive a key (hash) from the password and salt
            var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm);
            byte[] hash = key.GetBytes(KeySize);

            // Combine salt + hash into a single string
            var result = new byte[SaltSize + KeySize];
            Buffer.BlockCopy(salt, 0, result, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, result, SaltSize, KeySize);

            return Convert.ToBase64String(result);
        }

        public static bool Verify(string password, string hashedPassword)
        {
            byte[] decoded = Convert.FromBase64String(hashedPassword);

            byte[] salt = new byte[SaltSize];
            byte[] originalHash = new byte[KeySize];

            Buffer.BlockCopy(decoded, 0, salt, 0, SaltSize);
            Buffer.BlockCopy(decoded, SaltSize, originalHash, 0, KeySize);

            var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithm);
            byte[] attemptedHash = key.GetBytes(KeySize);

            return CryptographicOperations.FixedTimeEquals(originalHash, attemptedHash);
        }
    }
}
