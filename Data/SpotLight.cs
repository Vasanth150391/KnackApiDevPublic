using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Knack.DBEntities;

public partial class SpotLight
{
    [Key]
    public Guid? SpotlightId { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? UsecaseId { get; set; }

    public Guid? PartnerSolutionId { get; set; }

    public string? SpotlightOverview { get; set; }

    public string? RowChangedBy { get; set; }

    public DateTime? RowChangedDate { get; set; }
}
