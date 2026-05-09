namespace Knack.API.Interfaces
{
    public interface IReportBuilder
    {
        Task<string> GetTextReport();
    }
}
