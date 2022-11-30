using shuttleasy.DAL.EFRepositories.PasswordReset;
using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PasswordReset
{
    public class PasswordResetLogic : IPasswordResetLogic
    {
        private readonly IPasswordResetRepository _passwordResetRepository;
        public PasswordResetLogic(IPasswordResetRepository passwordResetRepository)
        {
            _passwordResetRepository = passwordResetRepository;
        }

        public bool Add(ResetPassword resetPassword)
        {
            bool isAdded = _passwordResetRepository.Add(resetPassword);
            return isAdded;
        }
        public ResetPassword GetResetPasswordWithEmail(string email) // yav buralara try catch yazmak lazım ama ne döndüreceğimi bilmiyom
        {
            Func<ResetPassword, bool> getResetPassword = getResetPassword => getResetPassword.Email == email;
            try
            {
                ResetPassword resetPassword = _passwordResetRepository.GetSingle(getResetPassword) ?? throw new ArgumentNullException();
                return resetPassword;
            }
            catch (Exception)
            {
                return new ResetPassword();
            }


        }
    }
}
