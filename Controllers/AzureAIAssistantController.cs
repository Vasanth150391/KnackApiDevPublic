using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Knack.API.Models;
using Knack.API.Interfaces;
using Knack.API.AzureAI;
using System.Text.RegularExpressions;

namespace Knack.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AzureAIAssistantController : ControllerBase
    {
        private readonly IAIAssistBuilder _aiAssistBuilder;
        private readonly ISqlQueryBuilder _sqlQueryOrchestrator;
        public AzureAIAssistantController(ISqlQueryBuilder sqlQueryOrchestrator, IAIAssistBuilder aiAssistBuilder)
        {
            _sqlQueryOrchestrator = sqlQueryOrchestrator;
            _aiAssistBuilder = aiAssistBuilder;
        }

        [HttpPost]
        [Route("GrammerCheck")]
        public async Task<IActionResult> GrammerCheck([FromBody] AIRequest aIRequest)
        {
            try
            {
                var response = await _aiAssistBuilder.GammerCheckerAgent(aIRequest.userPrompt[0]);
                var aiResponse = new AIResponse()
                {
                    Response = response
                };
                return Ok(aiResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Route("TextStatementAnalyzer")]
        public async Task<IActionResult> TextStatementAnalyzer([FromBody] AIRequest aIRequest, [FromQuery] string industry, [FromQuery] string subindustry)
        {
            try
            {
                var response = await _aiAssistBuilder.TextStatementAnalyzer(aIRequest.userPrompt, industry, subindustry);
                var aiResponse = new AIResponse()
                {
                    Response = response
                };
                return Ok(aiResponse);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost]
        [Route("ExecuteSqlQuery")]
        public async Task<IActionResult> ExecuteSqlQuery([FromBody] SqlQueryRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.SqlQuery))
                    return BadRequest("SQL query is required.");

                if (!IsSafeReadOnlySqlQuery(request.SqlQuery))
                    return BadRequest("Only single-statement read-only SELECT queries are allowed.");
                var result = await _sqlQueryOrchestrator.ExecuteQueryAsync(request.SqlQuery);
                return Ok(new { Result = result });
            }
            catch (Exception ex)
            {
                // Log the exception (ex) as needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while executing the SQL query.");
            }
        }
        private static bool IsSafeReadOnlySqlQuery(string sqlQuery)
        {
            var normalized = sqlQuery.Trim();

            if (!(Regex.IsMatch(normalized, @"^\s*select\b", RegexOptions.IgnoreCase) ||
                  Regex.IsMatch(normalized, @"^\s*with\b[\s\S]*\bselect\b", RegexOptions.IgnoreCase)))
                return false;

            if (normalized.Contains(";"))
                return false;

            if (Regex.IsMatch(normalized, @"(--|/\*|\*/)", RegexOptions.IgnoreCase))
                return false;

            if (Regex.IsMatch(
                normalized,
                @"\b(insert|update|delete|merge|drop|alter|create|truncate|exec|execute|grant|revoke|deny)\b",
                RegexOptions.IgnoreCase))
                return false;

            return true;
        }
        [HttpPost]
        [Route("ProcessUserPrompt")]
        public async Task<IActionResult> GenerateSqlQuery([FromBody] AIRequest request)
        {
            try
            {
                var aiResponse = new AISqlQueryResponse();
                aiResponse = await _sqlQueryOrchestrator.GenerateAISqlQueryAsync(request.userPrompt[0]);
                if (aiResponse != null && aiResponse.Sql == string.Empty)
                    return Ok(new { aiResponse });

                var result = await _sqlQueryOrchestrator.ExecuteQueryAsync(aiResponse.Sql);
                if (result is IEnumerable<object> collection)
                {
                    var lines = collection.Select(row => string.Join(", ", ((IDictionary<string, object>)row).Select(kv => $"{kv.Key}: {kv.Value}")));
                    aiResponse.ChatResponse = string.Join("\n", lines);
                }
                else
                {
                    aiResponse.ChatResponse = result?.ToString();
                }
                return Ok(new { aiResponse });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while generating the SQL query.");
            }
        }
    }
}