using Knack.DBEntities;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Knack.API.Models
{
    public class IndustryThemeDTO
    {
        public Guid IndustryId { get; set; }

        public Guid? SubIndustryId { get; set; }
        public Guid IndustryThemeId { get; set; }
        public Guid? PartnerId { get; set; }
        public string? IndustryName { get; set; } = null!;
        public string? SubIndustryName { get; set; } = null!;
        public string? Theme { get; set; } = null!;
        public string? Status { get; set; } = null!;
        public string? DisplayLabel { get; set; } = null!;
        public string? IsPublished { get; set; } = null!;
        public string? IndustryThemeDesc { get; set; } = null!;
        public string? Image_Thumb { get; set; } = null!;
        public string? Image_Main { get; set; } = null!;   
        public string? Image_Mobile { get; set; } = null!;
        public Guid? SolutionStatusId { get; set; }
        public string? IndustryThemeSlug { get; set; }
        public string? IndustrySlug { get; set; }
        public string? SubIndustrySlug { get; set; }
        public Boolean? showSubInd { get; set; }
    }

    public partial class IndustryDTO
    {
        [Key]
        public Guid IndustryId { get; set; }

        public string IndustryName { get; set; } = null!;

        public string? IndustryDescription { get; set; }

        public string Status { get; set; } = null!;

        public string? RowChangedBy { get; set; }

        public DateTime? RowChangedDate { get; set; }
        public Boolean? showInd { get; set; }
        public virtual List<IndustryThemeDTO>? SubIndustries { get; set; }
    }
    public partial class IndustryViewDTO
    {
        [Key]
        public Guid IndustryId { get; set; }

        public string IndustryName { get; set; } = null!;

        public string? IndustryDescription { get; set; }
        public string? ImageURL{ get; set; }
        public string? IndustrySlug { get; set; }
        public string? ImageMobileURL { get; set; }
        public List<IndustryThemeDTO>? SubIndustriesTheme { get; set; }
    }
    public class IndustryThemeDropDownDTO
    {        
        public Guid IndustryThemeId { get; set; }
        public string? Theme { get; set; } = null!;
    }
    public class IndustryDropDownDTO
    {
        public Guid IndustryId { get; set; }
    }
    public class IndustryReportDropDownDTO
    {
        public List<IndustryDropDownDTO>? IndustryId { get; set; }
        public List<IndustryThemeDropDownDTO>? IndustryThemeId { get; set; }
        public List<SolutionAreaDTO>? SolutionAreaId { get; set; }
        public Boolean? ExcludeSolutionArea { get; set; }
    }
    public class ArchiveIndustryReportDropDownDTO
    {        
        public List<IndustryThemeDropDownDTO>? IndustryThemeId { get; set; }
        public List<SolutionAreaDTO>? SolutionAreaId { get; set; }
        public Boolean? ExcludeSolutionArea { get; set; }
    }
    public class PartnerReportDropDownDTO
    {
        public List<SubIndustryDropDownDTO>? SubIndustryId { get; set; }
        public List<OrganizationsDropDown>? OrgId { get; set; }
        public List<SolutionStatusDropDownDTO>? SolutionStatusId { get; set; }
    }
    public class SubIndustryDropDownDTO
    {
        public Guid SubIndustryId { get; set; }
    }
    public class OrganizationsDropDown
    {
        public Guid OrgId { get; set; }
    }
    public class SolutionStatusDropDownDTO
    {
        public Guid SolutionStatusId { get; set; }
    }
}
