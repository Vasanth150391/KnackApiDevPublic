namespace Knack.API.Interfaces
{
    public interface IAIAssistManager
    {
        Task<string> TextStatementAnalyzer(string[] UserPrompt, string industry, string subindustry);
        Task<string> GammerCheckerAgent(string UserPrompt);
    }
}
