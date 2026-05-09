using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

[Table("ResourceLink")]
public partial class ResourceLink
{
    [Key]
    public Guid ResourceLinkId { get; set; }

    public string ResourceLinkName { get; set; } = null!;

    public string? ResourceLinkDescription { get; set; }

    public int? DisplayOrder { get; set; }
    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
