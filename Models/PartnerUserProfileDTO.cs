using Knack.DBEntities;

namespace Knack.API.Models
{
    public class PartnerUserProfileDTO
    {
        public List<PartnerUser>? PartnerUserDTOs { get; set; }
        public List<PartnerUserSolutionDTO>? PartnerUserSolutionDTOs { get; set; }
    }
    public class PartnerUserChangeProfileDTO
    {
        public Guid PartnerUserId { get; set; }
        public List<PartnerUserSolutionDTO>? PartnerUserSolutionDTOs { get; set; }
    }
    public class PartnerUserSolutionDTO
    {
        public Guid? PartnerSolutionId { get; set; }
        public Guid? UserId { get; set; }
        public Guid? IndustryId { get; set; }
        public Guid? SubIndustryId { get; set; }
        public string? IndustryName { get; set; }
        public string? SubIndustryName { get; set; }
        public string? SolutionName { get; set; }
        public string? Status { get; set; }
    }
}