﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Services.AppointmentService
{
    public class EmailSender:IEmailSender
    {
        string ApiKey = Environment.GetEnvironmentVariable("API_KEY");
        

        




        public async Task SendMail(string subject,string toEmail,string userName,string htmlContent)
        {
            var apiKey = ApiKey;
            if (ApiKey == null)
            {
                apiKey = "aactest";
                
            }
           
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("chathuraishara63@gmail.com", "Medicare Hub");
           
            var to = new EmailAddress(toEmail, userName);
            var plainTextContent = "";
           





            try
            {
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);


            }catch (Exception ex)
            {
                throw new Exception("Email sending failed");
            }



        }


    }
}
