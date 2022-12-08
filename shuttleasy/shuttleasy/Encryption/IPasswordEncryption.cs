using shuttleasy.DAL.Models;

namespace shuttleasy.Encryption
{
    public interface IPasswordEncryption
    {
        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(byte[] passwordHash, byte[] passwordSalt, string password);
        public bool ResetPassengerPassword(string password, Passenger passenger);
        public bool ResetDriverPassword(string password, CompanyWorker companyWorker);
        
    }
}
