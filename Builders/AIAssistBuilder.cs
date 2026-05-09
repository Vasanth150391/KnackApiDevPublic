using System.Threading.Tasks;
using Knack.API.Interfaces;
using Knack.API.AzureAI;

namespace Knack.API.Builders
{
    public class AIAssistBuilder : IAIAssistBuilder
    {
        private readonly AzureAIAssistantClient _azureAIAssistantClient;

        public AIAssistBuilder(AzureAIAssistantClient azureAIAssistantClient)
        {
            _azureAIAssistantClient = azureAIAssistantClient;
        }

        public async Task<string> TextStatementAnalyzer(string[] UserPrompt, string industry, string subindustry)
        {
            return await _azureAIAssistantClient.TextStatementAnalyzer(UserPrompt, industry, subindustry);
        }

        public async Task<string> GammerCheckerAgent(string UserPrompt)
        {
            return await _azureAIAssistantClient.GammerCheckerAgent(UserPrompt);
        }
    }
}
