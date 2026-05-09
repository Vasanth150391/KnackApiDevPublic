using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Knack.DBEntities;

[Table("ArchiveSubIndustry")]
public partial class ArchiveSubIndustry
{
    [Key, ForeignKey("Industry")]
    public Guid SubIndustryId { get; set; }

    public Guid IndustryId { get; set; }

    public string? SubIndustryName { get; set; } = null!;

    public string? SubIndustryDescription { get; set; }

    public string? Status { get; set; } = null!;

    public string? SubIndustrySlug { get; set; } = null!;

    public Guid? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
