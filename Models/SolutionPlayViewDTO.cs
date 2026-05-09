using Knack.DBEntities;

namespace Knack.API.Models
{
    public class SolutionPlayViewDTO
    {
        public Guid? SolutionPlayId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? SolutionPlayThemeSlug { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? SolutionAreaName { get; set; }
        public string? SolutionAreaSlug { get; set; }
        public string? SolutionPlayDesc { get; set; }
        public string? SolutionPlayLabel { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public Guid? RowChangedBy { get; set; }
        public DateTime? RowChangedDate { get; set; }
        public string? ImageThumb { get; set; } = null!;
        public string? Image_Main { get; set; } = null!;
        public string? Image_Mobile { get; set; } = null!;
        public int? IsPublished { get; set; }
        public List<TechnologyShowcasePartnerSolutionDTO> PartnersSolutions { get; set; }
    }
}