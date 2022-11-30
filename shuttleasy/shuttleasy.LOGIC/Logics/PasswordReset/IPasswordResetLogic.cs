using shuttleasy.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shuttleasy.LOGIC.Logics.PasswordReset
{
    public interface IPasswordResetLogic
    {
        public bool Add(ResetPassword resetPassword);
        public ResetPassword GetResetPasswordWithEmail(string email);
    }
}
