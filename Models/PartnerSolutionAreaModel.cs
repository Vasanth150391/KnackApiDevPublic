namespace Knack.API.Models
{
    public class PartnerSolutionAreaModel
    {
        public Guid PartnerSolutionByAreaId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? AreaSolutionDescription { get; set; }
        public Guid? PartnerSolutionId { get; set; }
        public List<PartnerSolutionResourceLinkModel> PartnerSolutionResourceLinks { get; set; }
    }
}
