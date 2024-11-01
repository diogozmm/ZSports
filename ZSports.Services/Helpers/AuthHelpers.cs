using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZSports.Core.ViewModel.User;
using ZSports.Domain.User;

namespace ZSports.Services.Helpers
{
    public static class AuthHelpers
    {
        public static byte[] HashPassword(string password, byte[] salt, int iterations = 10000, int hashSize = 32)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return rfc2898.GetBytes(hashSize); // Hash of password + salt
            }
        }

        private static byte[] GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] HashPasswordWithSalt(string password, int saltSize = 16, int iterations = 10000, int hashSize = 32)
        {
            byte[] salt = GenerateSalt(saltSize);
            byte[] hash = HashPassword(password, salt, iterations, hashSize);

            // Combine salt and hash into a single byte array
            byte[] hashBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

            return hashBytes; // Directly return byte array instead of Base64 string
        }


        public static bool VerifyPassword(string password, byte[] storedHash, int saltSize = 16, int iterations = 10000, int hashSize = 32)
        {
            // Extract salt from the stored hash
            byte[] salt = new byte[saltSize];
            Array.Copy(storedHash, 0, salt, 0, salt.Length);

            // Compute hash from provided password
            byte[] hash = HashPassword(password, salt, iterations, hashSize);

            // Compare the stored hash with the computed hash
            for (int i = 0; i < hash.Length; i++)
            {
                if (storedHash[i + salt.Length] != hash[i])
                    return false;
            }
            return true;
        }

        public static LoginResponse GenerateToken(User user, string secret)
        {
            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret!);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                }),
                Expires = DateTime.UtcNow.AddHours(1), // Set token expiration
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new LoginResponse
            {
                Token = tokenHandler.WriteToken(token)
            };
        }

    }
}
