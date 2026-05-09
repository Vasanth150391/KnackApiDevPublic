using System;
using System.Collections.Generic;

namespace Knack.DBEntities;

public partial class PartnerSolutionPlay
{
    public Guid PartnerSolutionPlayId { get; set; }

    public Guid SolutionAreaId { get; set; }

    public Guid? OrgId { get; set; }

    public string? PartnerSolutionPlaySlug { get; set; }

    public string? SolutionPlayName { get; set; }

    public string? SolutionPlayDescription { get; set; }

    public string? SolutionPlayOrgWebsite { get; set; }

    public string? MarketplaceLink { get; set; }

    public string? SpecialOfferLink { get; set; }

    public string? LogoFileLink { get; set; }

    public Guid? SolutionStatusId { get; set; }

    public Guid? RowChangedBy { get; set; }

    public int? IsPublished { get; set; }

    public DateTime? RowChangedDate { get; set; }
    public string? Image_Thumb { get; set; }
    public string? Image_Main { get; set; }
    public string? Image_Mobile { get; set; }
    public int? IndustryDesignation { get; set; }
}
