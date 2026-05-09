using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerSolutionResourceLink
{
    public Guid PartnerSolutionResourceLinkId { get; set; } 

    public Guid PartnerSolutionByAreaId { get; set; } 

    public Guid ResourceLinkId { get; set; } 

    public string ResourceLinkTitle { get; set; } = null!;

    public string ResourceLinkUrl { get; set; } = null!;
    public string? ResourceLinkOverview { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }

    public DateTime? EventDateTime { get; set; }
}
