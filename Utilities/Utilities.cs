using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Knack.API.Models;
using Microsoft.Extensions.Configuration;
using Knack.API.Data;
using System.Text.RegularExpressions;
using Knack.DBEntities;

namespace Knack.API.Common
{
    public class Utilities
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<Utilities> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _basefilePath = string.Empty;

        /// <summary>
        /// Constructor with single parameter.
        /// </summary>
        /// <param name="iConfig"></param>
        public Utilities(IConfiguration iConfig)
        {
            _configuration = iConfig;
            _basefilePath = Directory.GetCurrentDirectory() + "\\Mail_Attachment";
        }

        /// <summary>
        /// Constructor with two parameter.
        /// </summary>
        /// <param name="iConfig"></param>
        /// <param name="logger"></param>
        public Utilities(IConfiguration iConfig,ILogger<Utilities> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = iConfig;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public Utilities()
        {
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public string EncryptString(string key, string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        /// <summary>
        /// Decrypting the string.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sending email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="emailBody"></param>
        /// <param name="subject"></param>
        public void PrepareEmail(string subject,string partnerEmail,string emailBody)
        {
            var link = "https://partner.microsoftindustryinsights.com/industrySolution";

            if (_httpContextAccessor.HttpContext.Request.Host.Host.Contains("dev"))
            {
                link = "https://mssoldir-app-dev.azurewebsites.net/industrySolution";
            }
            else if (_httpContextAccessor.HttpContext.Request.Host.Host.Contains("qa"))
            {
                link = "https://mssoldir-app-qa.azurewebsites.net/industrySolution";
            }

           
            string encKey = _configuration.GetSection("KnackSettings").GetSection("EncKey").Value;
            var partnerEmailAddress = DecryptString(encKey, partnerEmail);

            emailBody = emailBody.Replace("#link#", link);
            SendMail(partnerEmailAddress, subject, emailBody);
        }

        /// <summary>
        /// sending email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendMail(string emailAddress,string subject,string body)
        {
            var smtpServer = _configuration.GetSection("EmailSettings").GetSection("SmtpServer").Value;
            var smtpPort = 587; // Port 587 for TLS
            var smtpUsername = _configuration.GetSection("EmailSettings").GetSection("UserName").Value;
            var smtpPassword = _configuration.GetSection("EmailSettings").GetSection("Pwd").Value;   
            var senderEmail= _configuration.GetSection("EmailSettings").GetSection("SenderEmail").Value;
            // Sender and recipient email addresses           

            var senderEmailAddress = new MailAddress(senderEmail, "Knack");
            var recipientEmail = new MailAddress(emailAddress);

            // Create a new message
            var message = new MailMessage(senderEmailAddress, recipientEmail);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;
            
            // Setup SMTP client
            var smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.EnableSsl = true; // Enable SSL/TLS
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            try
            {
                //System.Diagnostics.Trace.WriteLine($"EmailAddress {emailAddress},Email Subject: {subject}, Email Body: {body}");                
                // Send email
                smtpClient.Send(message);
                //System.Diagnostics.Trace.WriteLine($"Email sent successfully.");
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Failed to send email: {ex.Message}");
                throw ex;
              
            }
            return true;
        }
        /// <summary>
        /// sending email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public bool SendLeadMail(string emailAddress, string subject, string body, bool attachment)
        {
            var smtpServer = _configuration.GetSection("EmailSettings").GetSection("SmtpServer").Value;
            var smtpPort = 587; // Port 587 for TLS
            var smtpUsername = _configuration.GetSection("EmailSettings").GetSection("UserName").Value;
            var smtpPassword = _configuration.GetSection("EmailSettings").GetSection("Pwd").Value;
            var senderEmail = _configuration.GetSection("EmailSettings").GetSection("SenderEmail").Value;
            // Sender and recipient email addresses           

            var senderEmailAddress = new MailAddress(senderEmail, "Knack");
            var recipientEmail = new MailAddress(emailAddress);

            // Create a new message
            var message = new MailMessage(senderEmailAddress, recipientEmail);
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = body;

            if (attachment == true)
            {
                var SamplePdf = _basefilePath + "\\LeadCreationFormat_Sample.pdf";
                message.Attachments.Add(new Attachment(SamplePdf));
                var SampleCSV = _basefilePath + "\\LeadDoc.csv";
                message.Attachments.Add(new Attachment(SampleCSV));
                var pythonDoc = _basefilePath + "\\Upload File to SFTPGO Using Python.pdf";
                message.Attachments.Add(new Attachment(pythonDoc));
            }
            // Setup SMTP client
            var smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.EnableSsl = true; // Enable SSL/TLS
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);

            try
            {
                //System.Diagnostics.Trace.WriteLine($"EmailAddress {emailAddress},Email Subject: {subject}, Email Body: {body}");
                // Send email
                smtpClient.Send(message);
                System.Diagnostics.Trace.WriteLine($"Email sent successfully.");
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Failed to send email: {ex.Message}");
                throw ex;

            }
            return true;
        }
        /// <summary>
        /// Creating the unique id for partnersolution records.
        /// </summary>
        /// <param name="solutionName"></param>
        /// <returns></returns>
        public string GenerateSlug(string solutionName)
        {
            try
            {
                _logger.LogDebug("Entering the method GenerateSlug");

                string slug = Regex.Replace(solutionName, @"<[^>]+>|&nbsp;", "").Trim();
                slug = Regex.Replace(slug, @"\s{2,}", " ");
                slug = Regex.Replace(slug, "[^0-9A-Za-z _-]", "");
                slug = Regex.Replace(slug, @" ", "-");
                slug = slug.ToLower();
                Boolean slugCheck = true;
                string slugTmp = slug;
                //while (slugCheck)
                //{
                //    var resCheck = _knackContext.PartnerSolution.Where(t => t.PartnerSolutionSlug == slug).
                //        Where(t => t.PartnerSolutionId != partnerSolution.PartnerSolutionId).FirstOrDefault();
                //    if (resCheck == null)
                //        slugCheck = false;
                //    else
                //    {
                Random rnd = new Random();
                int j = rnd.Next(1, 100003);
                slug = slugTmp + "-" + j;
                // }
                //}
                return slug;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred in method GenerateSlug {ex.Message}");
                throw;
            }
        }
    }
}
