using Knack.DBEntities;

namespace Knack.API.Models
{
    public class SolutionAreaDTO
    {
       public Guid SolutionAreaId { get; set; }

        public string? SolutionAreaName { get; set; } = null!;
        public string? SolutionAreaSlug { get; set; } = null!;
        public string? SolAreaDescription { get; set; }
        public string? ImageThumb { get; set; } = null!;
        public string? ImageMain { get; set; } = null!;
        public string? ImageMobile { get; set; } = null!;
    }
}
