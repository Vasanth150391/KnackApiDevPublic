using System.Threading.Tasks;
using Knack.API.Interfaces;
using Knack.API.Models;

namespace Knack.API.Builders
{
    public class SqlQueryBuilder : ISqlQueryBuilder
    {
        private readonly ISqlQueryManager _sqlQueryManager;
        private readonly Knack.API.AzureAI.AzureAIAssistantClient _azureAIAssistantClient;

        public SqlQueryBuilder(ISqlQueryManager sqlQueryManager, Knack.API.AzureAI.AzureAIAssistantClient azureAIAssistantClient)
        {
            _sqlQueryManager = sqlQueryManager;
            _azureAIAssistantClient = azureAIAssistantClient;
        }

        public async Task<object> ExecuteQueryAsync(string sqlQuery)
        {
            return await _sqlQueryManager.ExecuteQueryAsync(sqlQuery);
        }

        public async Task<AISqlQueryResponse> GenerateAISqlQueryAsync(string userPrompt)
        {
            return await _azureAIAssistantClient.GenerateSqlQueryFromNaturalLanguage(userPrompt);
        }
    }
}
