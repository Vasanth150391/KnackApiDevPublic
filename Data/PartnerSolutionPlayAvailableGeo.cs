using System.ComponentModel.DataAnnotations;

namespace Knack.DBEntities;

public partial class PartnerSolutionPlayAvailableGeo
{
    [Key]
    public Guid? PartnerSolutionPlayAvailableGeoId { get; set; } = null!;

    public Guid? PartnerSolutionPlayId { get; set; } = null!;

    public Guid? GeoId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
