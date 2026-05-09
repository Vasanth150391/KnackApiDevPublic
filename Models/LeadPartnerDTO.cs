using System.ComponentModel.DataAnnotations;

namespace Knack.API.Models
{
    public class LeadPartnerDTO
    {
        [Required(ErrorMessage ="FirstName field cannot be empty.")]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required(ErrorMessage = "EmailAddress field cannot be empty.")]
        public string EmailAddress { get; set; }
        public string? FTPUserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string CompanyName { get; set; }
        public string? Status { get; set; }
    }
}
