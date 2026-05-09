using Knack.DBEntities;
using System.ComponentModel.DataAnnotations;
namespace Knack.API.Models
{
    public class OrganizationDTO
    {
        public Guid? OrgId { get; set; }
        public string? OrgName { get; set; }
        public string? logoFileLink { get; set; }
        public string? orgWebsite { get; set; }
    }
    public class CheckOrganizationDTO
    {
        public Boolean? orgExists { get; set; }
    }
    public partial class AdminOrganizationDTO
    {
        public Guid? OrgId { get; set; } = null!;

        public string OrgName { get; set; } = null!;

        public string? OrgDescription { get; set; }

        public string Status { get; set; } = null!;

        public string? RowChangedBy { get; set; }

        public DateTime? RowChangedDate { get; set; }

        public string? LogoFileLink { get; set; }

        public string? OrgWebsite { get; set; }
        public string? TotalSolution { get; set; }

    }
}
