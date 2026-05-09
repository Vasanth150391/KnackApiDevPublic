using Knack.DBEntities;
using System.ComponentModel.DataAnnotations;

namespace Knack.API.Models
{
    public class SolutionAreaPlayDTO
    {
        public Guid SolutionAreaId { get; set; }

        public string? SolutionAreaName { get; set; } = null!;
        public string? SolutionAreaSlug { get; set; } = null!;
        public string? SolutionAreaDescription { get; set; }
        public string? ImageThumb { get; set; } = null!;
        public string? ImageMain { get; set; } = null!;
        public string? ImageMobile { get; set; } = null!;

        public List<SolutionPlayDTO>? SolutionPlays { get; set; }
    }
    public class SolutionPlayDTO
    {
        public Guid? SolutionPlayId { get; set; }
        public Guid? SolutionAreaId { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? SolutionAreaName { get; set; }
        public Guid? SolutionStatusId { get; set; }
        public string? SolutionStatus { get; set; }
        public string? DisplayLabel { get; set; }
        public string? SolutionPlayDesc { get; set; }
        public int? IsPublished { get; set; }
        public string? SolutionPlayLabel { get; set; }
        public Guid? RowChangedBy { get; set; }
        public DateTime? RowChangedDate { get; set; }
        public string? ImageThumb { get; set; } = null!;
        public string? ImageMain { get; set; } = null!;
        public string? ImageMobile { get; set; } = null!;
        public int? totalSolutionPlay { get; set; }
        public string SolutionPlayThemeSlug { get; set; }

        public List<TechnologyShowcasePartnerSolutionDTO> PartnersSolutions { get; set; }
    }
}
