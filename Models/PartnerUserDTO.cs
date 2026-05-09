namespace Knack.API.Models
{
    public class PartnerUserDTO
    {
        public Guid PartnerUserId { get; set; }
        public Guid OrgId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string PartnerEmail { get; set; }
        public string? PartnerTitle { get; set; }
        public string? UserType { get; set; }
        public string? Route_PostAuthentication { get; set; }
    }
    public class PartnerUserAdminDTO
    {
        public Guid? PartnerUserId { get; set; }
        public Guid? OrgId { get; set; }
        public string? OrgName { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string PartnerEmail { get; set; }
        public string PartnerTitle { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
    }
}
