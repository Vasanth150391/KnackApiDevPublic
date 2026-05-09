using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerSolutionPlayByPlay
{
    public Guid PartnerSolutionPlayByPlayId { get; set; }

    public Guid PartnerSolutionPlayId { get; set; } 

    public Guid? SolutionPlayId { get; set; } 

    public string? PlaySolutionDescription { get; set; } = null!;

    public string? Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
