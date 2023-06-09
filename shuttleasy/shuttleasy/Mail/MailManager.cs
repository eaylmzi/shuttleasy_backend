﻿using MailKit.Security;
using MimeKit.Text;
using MimeKit;
using MailKit.Net.Smtp;
using shuttleasy.LOGIC.Logics;

namespace shuttleasy.Mail
{
    public class MailManager : IMailManager
    {
        private readonly IPassengerLogic _passengerLogic;
        private readonly IConfiguration _configuration;
        public MailManager(IPassengerLogic passengerLogic, IConfiguration configuration)
        {
            _passengerLogic = passengerLogic;
            _configuration = configuration;
        }

        public bool sendMail(string emailAddress ,string subject,string body, IConfiguration configuration)
        {
            try
            {
                
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(
                     configuration.GetSection("CompanyEmail:Email").Value ?? throw new ArgumentNullException()
                    ));
                email.To.Add(MailboxAddress.Parse(emailAddress));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = "<p>"+body+"</p>"
                    //"<a href=\"https://www.google.com\">"+body+"</a>"
                };

                using var smtp = new SmtpClient();
                smtp.Connect("smtp.office365.com", 587, SecureSocketOptions.StartTls);
                smtp.Authenticate(
                     configuration.GetSection("CompanyEmail:Email").Value ?? throw new ArgumentNullException(),
                     configuration.GetSection("CompanyEmail:Password").Value ?? throw new ArgumentNullException()
                     );
                smtp.Send(email);
                smtp.Disconnect(true);
                return true;

            }
            catch (Exception) //shuttleasy2spawn atıyo amk düzelt
            {
                return false;
            }
            
           
        }
        public void notifyPassengersPaymentDay()
        {
            var passengersList = _passengerLogic.GetAllPassengers();
            foreach (var passenger in passengersList) //Observer pattern yapçan 
            {
                sendMail(passenger.Email ,"d","s",_configuration); //BURAYI DÜZELT
            }

        }

        
    }
}
