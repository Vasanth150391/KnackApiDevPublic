namespace Knack.API.Models
{
    public class SolutionStatusDTO
    {
        public Guid? SolutionStatusId { get; set; }

        public string SolutionStatus { get; set; } = null!;
        public string DisplayLabel { get; set; } = null!;
    }
}
