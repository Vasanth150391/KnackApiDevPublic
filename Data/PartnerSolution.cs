using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerSolution
{
    public Guid PartnerSolutionId { get; set; }

    public Guid UserId { get; set; }

    public Guid? IndustryId { get; set; }

    public Guid? SubIndustryId { get; set; }

    public Guid? OrganizationId { get; set; }
    public string? PartnerSolutionSlug { get; set; }
    public string? SolutionName { get; set; }

    public string? SolutionDescription { get; set; }

    public string? SolutionOrgWebsite { get; set; }

    public string? MarketplaceLink { get; set; }

    public string? SpecialOfferLink { get; set; }

    public string? LogoFileLink { get; set; }

    public Guid? SolutionStatusId { get; set; }

    public Guid? RowChangedBy { get; set; }

    public int? IsPublished { get; set; }
    public int? IndustryDesignation { get; set; }
    public DateTime? RowChangedDate { get; set; }
    public DateTime? RowCreatedDate { get; set; }
    public string? ParentSolutionId { get; set; }
}
