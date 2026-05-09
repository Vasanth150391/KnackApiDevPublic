using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;
[Table("Geo")]
public partial class Geo
{
    [Key]
    public Guid? GeoId { get; set; } = null!;

    public string Locale { get; set; } = null!;

    public string Geoname { get; set; } = null!;

    public string? Geodescription { get; set; }

    public int? DisplayOrder { get; set; }
    
    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
