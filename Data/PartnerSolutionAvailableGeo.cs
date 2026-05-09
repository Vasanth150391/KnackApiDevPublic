using System.ComponentModel.DataAnnotations;

namespace Knack.DBEntities;

public partial class PartnerSolutionAvailableGeo
{
    [Key]
    public Guid? PartnerSolutionAvailableGeoId { get; set; } = null!;

    public Guid? PartnerSolutionId { get; set; } = null!;

    public Guid? GeoId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
