using Knack.DBEntities;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Knack.API.Models
{

    public partial class MenuIndustryDTO
    {
        [Key]
        public Guid IndustryId { get; set; }
        public Guid? IndustryThemeId { get; set; }
        public string IndustryName { get; set; } = null!;
        public bool hasMultipleThme { get; set; }
        public bool hasSubMenu { get; set; }
        public string? IndustryThemeSlug { get; set; }
        public string? IndustrySlug { get; set; }
        public virtual List<MenuSubIndustryDTO>? SubIndustries { get; set; }
    }

    public class MenuSubIndustryDTO
    {
        public Guid? SubIndustryId { get; set; }
        public string? SubIndustryName { get; set; } = null!;
        public string? SubIndustrySlug { get; set; } = null!;
        public Guid? IndustryThemeId { get; set; }
        public string? IndustryThemeSlug { get; set; }
        public List<MenuSolutionAreaDTO>? SolutionAreas { get; set; }
    }

    public partial class MenuSolutionAreaDTO
    {
        
        public Guid IndustryThemeId { get; set; }
        public Guid? SolutionAreaId { get; set; } = null!;
        public string? solutionAreaName { get; set; } = null!;
        public string? IndustryThemeSlug { get; set; }
    }
}
