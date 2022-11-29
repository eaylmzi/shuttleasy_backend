namespace shuttleasy.Mail
{
     public interface IMailManager
    {
        public bool sendMail(string emailAddress, string subject, string body, IConfiguration _configuration);
        public void notifyPassengersPaymentDay();
    }
}
