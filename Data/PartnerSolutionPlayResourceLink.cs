using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerSolutionPlayResourceLink
{
    public Guid PartnerSolutionPlayResourceLinkId { get; set; } 

    public Guid? PartnerSolutionPlayByPlayId { get; set; } 

    public Guid ResourceLinkId { get; set; } 

    public string ResourceLinkTitle { get; set; } = null!;

    public string ResourceLinkUrl { get; set; } = null!;
    
    public string? ResourceLinkOverview { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
