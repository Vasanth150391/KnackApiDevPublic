namespace Knack.API.Models
{
    public class PartnerSolutionResourceLinkModel
    {
        public Guid? ResourceLinkId { get; set; }
        public Guid PartnerSolutionResourceLinkId { get; set; }
        public string ResourceLinkTitle { get; set; }
        public string ResourceLinkUrl { get; set; }
        public string? ResourceLinkOverview { get; set; }
        public Guid? PartnerSolutionByAreaId { get; set; }
        public DateTime? EventDateTime { get; set; }
        public Guid? Status { get; set; }
    }
}
