namespace Knack.API.Models
{
    public class RecipientOTPMailDTO
    {
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string OTP { get; set; }
        public string smtpServer { get; set; }
        public string smtpUsername { get; set; }
        public string smtpPassword { get; set; }

    }
}
