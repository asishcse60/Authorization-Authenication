using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;

using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ConfArch.Web.Areas.Identity
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        //SendGrid Config
        private readonly string _apiKey;
        private readonly string _fromName;
        private readonly string _fromEmail;

        //Twilio Config
        private readonly string _smsAccountSid;
        private readonly string _authToken; 
        private readonly string _adminPhone;

        //AWS Config
        private readonly string _accessKey; 
        private readonly string _secretKey; 
        private readonly string _regionName; 
        public AuthMessageSender(IConfiguration config)
        {
            //SendGrid Config
            _apiKey = config["SendGrid:ApiKey"];
            _fromName = config["SendGrid:FromName"];
            _fromEmail = config["SendGrid:FromEmail"];

            //Twilio Config
            _smsAccountSid = config["Twilio:AccountSid"];
            _authToken = config["Twilio:AuthToken"];
            _adminPhone = config["Twilio:AdminPhone"];

            //Amazon Config
            _accessKey = config["MailConfig:SES:AwsSESAccessKey"];
            _secretKey = config["MailConfig:SES:AwsSESSecretKey"];
            _regionName = config["MailConfig:SES:AwsSESRegion"];

        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail),
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8,
                Body = message,
                Subject = subject
            };
            mailMessage.To.Add(new MailAddress(email));

            AWSCredentials awsCredentials = new BasicAWSCredentials(_accessKey, _secretKey);

            var awsconfig = new AmazonSimpleEmailServiceConfig
            {
                RegionEndpoint = RegionEndpoint.GetBySystemName(_regionName)
            };
            var rawMessage = new RawMessage();

            using (var memoryStream = ConvertMailMessageToMemoryStream(mailMessage))
            {
                rawMessage.Data = memoryStream;
            }

            var sendRequest = new SendRawEmailRequest
            {
                Source = _fromEmail,
                RawMessage = rawMessage,
                Destinations = new List<string>() { email}
            };
            var sesClient = new AmazonSimpleEmailServiceClient(awsCredentials, awsconfig);
            try
            {
                var response = sesClient.SendRawEmailAsync(sendRequest).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public Task SendSmsAsync(string customerNumber, string message)
        {
            TwilioClient.Init(_smsAccountSid, _authToken);

            var response = MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_adminPhone),
                to: new Twilio.Types.PhoneNumber(customerNumber)
            );
            return response;
        }
        private static MemoryStream ConvertMailMessageToMemoryStream(MailMessage message)
        {
            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");
            var stream = new MemoryStream();
            var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);
            var mailWriter = mailWriterContructor.Invoke(new object[] { stream });
            var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);
            sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { mailWriter, true, true }, null);
            return stream;
        }
    }
}
