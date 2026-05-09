using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class Solution
{
    public string SolutionId { get; set; } = null!;

    public string PartnerSolutionId { get; set; } = null!;

    public string IndustryId { get; set; } = null!;

    public string SubIndustryId { get; set; } = null!;

    public string SolutionAreaId { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? RowChangedBy { get; set; }

    public DateOnly? RowChangedDate { get; set; }
}
