namespace shuttleasy.Mail
{
     public interface IMailManager
    {
        public bool sendMail(string emailAddress, IConfiguration _configuration);
        public void notifyPassengersPaymentDay();
    }
}
