namespace Knack.API.Models
{
    public class OrganizationPartnerUserDTO
    {
        public Guid OrgId { get; set; }
        public string OrgName { get; set; }
        public Guid PartnerUserId { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string PartnerEmail { get; set; }
        public string PartnerTitle { get; set; }

        public string? UserType { get; set; } = null!;
    }
}
