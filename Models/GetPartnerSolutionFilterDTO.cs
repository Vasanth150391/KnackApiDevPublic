namespace Knack.API.Models
{
    public class GetPartnerSolutionFilterDTO
    {
        public Guid? SolutionAreaId { get; set; }
        public string? SolutionAreaName { get; set; }
        public List<GetPartnerSolutionDTO>? PartnerSolutions { get; set; }
    }
    
}
