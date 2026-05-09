namespace Knack.API.Models
{
    public class TechnologyShowcasePartnerSolutionDTO
    {
        public Guid? TechnologyShowcasePartnerSolutionId { get; set; }

        public Guid? SolutionPlayId { get; set; } = null!;

        public Guid? PartnerId { get; set; } = null!;

        public string PartnerName { get; set; } = null!;

        public string MarketPlaceLink { get; set; } = null!;

        public string? Status { get; set; } = null!;
        public string? websiteLink { get; set; } = null!;
        public string? overviewDescription { get; set; } = null!;
        public string? logoFileLink { get; set; } = null!;
        public string? RowChangedBy { get; set; }
        public string? PartnerNameSlug { get; set; }
        public DateTime? RowChangedDate { get; set; }
    }

    public class TechnologyShowcasePartnerSolutionViewDTO
    {
        public Guid? TechnologyShowcasePartnerSolutionId { get; set; }

        public Guid? SolutionPlayId { get; set; } = null!;

        public Guid? PartnerId { get; set; } = null!;

        public string PartnerName { get; set; } = null!;

        public string MarketPlaceLink { get; set; } = null!;

        public string? Status { get; set; } = null!;
        public string? websiteLink { get; set; } = null!;
        public string? overviewDescription { get; set; } = null!;
        public string? logoFileLink { get; set; } = null!;
        public string? RowChangedBy { get; set; }
        public string? PartnerNameSlug { get; set; }
        public DateTime? RowChangedDate { get; set; }
        public string? SolutionAreaName { get; set; }
        public string? SolutionPlayName { get; set; }
        public string? SolutionPlaySlug { get; set; }
        public string? SolutionAreaSlug { get; set; }
        public Guid? SolutionAreaId { get; set; } = null!;

    }
}
