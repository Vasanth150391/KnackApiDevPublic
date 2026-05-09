using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("IndustryShowcasePartnerSolution")]
public partial class IndustryShowcasePartnerSolution
{
    [Key]
    public Guid? IndustryShowcasePartnerSolutionId { get; set; } = null!;

    public Guid? IndustryThemeId { get; set; } = null!;

    public Guid? PartnerId { get; set; } = null!;

    public string PartnerName { get; set; } = null!;

    public string MarketPlaceLink { get; set; } = null!;

    public string Status { get; set; } = null!;
    public string? websiteLink { get; set; } = null!;
    public string? overviewDescription { get; set; } = null!;
    public string? logoFileLink { get; set; } = null!;
    public string? RowChangedBy { get; set; }
    public string? PartnerNameSlug { get; set; }
    public DateTime? RowChangedDate { get; set; }
}

