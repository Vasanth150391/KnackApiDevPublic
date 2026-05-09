using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerSolutionByArea
{
    public Guid PartnerSolutionByAreaId { get; set; }

    public Guid PartnerSolutionId { get; set; } 

    public Guid? SolutionAreaId { get; set; } 

    public string? AreaSolutionDescription { get; set; }

    public string? Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
