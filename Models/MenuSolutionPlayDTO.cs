using Knack.DBEntities;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace Knack.API.Models
{   

    public partial class MenuSolutionAreaListDTO
    {
        public Guid? SolutionAreaId { get; set; } = null!;
        public string? SolutionAreaName { get; set; } = null!;
        public string? SolutionAreaSlug { get; set; } = null!;
        public bool? hasSubMenu { get; set; }
        public virtual List<MenuSolutionPlayListDTO>? SolutionPlays { get; set; }
    }
    public partial class MenuSolutionPlayListDTO
    {
        public Guid? SolutionPlayId { get; set; } = null!;
        public Guid? SolutionAreaId { get; set; } = null!;
        public string? SolutionPlayName { get; set; } = null!;
        public string? SolutionPlayThemeSlug { get; set; } = null!;
        public string? SolutionPlayLabel { get; set; } = null!;
    }
}
