using shuttleasy.DAL.Models;
using System.Security.Cryptography;

namespace shuttleasy.Encryption
{
    public class PasswordEncryption
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }
        public bool VerifyPasswordHash(Passenger passenger, string password)
        {
            using (var hmac = new HMACSHA512(passenger.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                bool isMatched = computedHash.SequenceEqual(passenger.PasswordHash);
                return isMatched;
            }
        }
    }
}
