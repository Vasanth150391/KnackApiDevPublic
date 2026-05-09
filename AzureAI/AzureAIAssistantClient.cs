using Azure;
using Azure.AI.OpenAI;
using Knack.API.DataManagers;
using Knack.API.Interfaces;
using Knack.API.Models;
using Microsoft.Data.SqlClient;
using OpenAI.Chat;
using System.Text.Json;

namespace Knack.API.AzureAI
{
    public class AzureAIAssistantClient
    {
        private readonly ISqlQueryManager _sqlQueryManager;

        public AzureAIAssistantClient(ISqlQueryManager sqlQueryManager)
        {
            _sqlQueryManager = sqlQueryManager;
        }

        public ChatClient ConnectAzureAI()
        {
            string endpoint = @"https://openai-demo-eastus.openai.azure.com/";
            string apiKey = "9FlhpQUwDzLFGv9jhs4OENLaz3h6egN0WbuR8pTcz3gBQ96bp96uJQQJ99BHACYeBjFXJ3w3AAABACOGMdWo";
            string deploymentName = "gpt4-demo"; // Your deployment name
                                                 // --- Client Initialization ---
            var client = new AzureOpenAIClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
            var chatClient = client.GetChatClient(deploymentName);
            return chatClient;
        }

        public async Task<AISqlQueryResponse> GenerateSqlQueryFromNaturalLanguage(string userPrompt)
        {
            var schemaJson = await _sqlQueryManager.GetSchemaAsync();
            var chatClient = ConnectAzureAI(); // Await the Task to get ChatClient
            var chatHistory = new List<ChatMessage>
            {
                //new SystemChatMessage("You are a grammar correction assistant. "+ "Provide only the corrected version of the text.")
                //new SystemChatMessage("You are a grammar, spelling and formatting assistant. When given text, do exactly the following and nothing else: " + "1) Produce a section that contains the corrected version of the user's text preserving the original paragraph breaks and overall formatting (do not merge paragraphs). " + "2) Do NOT include any explanations or extra commentary.")                
                /*new SystemChatMessage("You are a grammar, spelling and formatting assistant. When given text, do exactly the following and nothing else: "
                + "1) Produce a section that contains the corrected version of the user's text preserving the original paragraph breaks and overall formatting (do not merge paragraphs). "
                + "2) Do NOT include any explanations or extra commentary."
                + "3) For each paragraph, correct grammar, punctuation, and spelling, then choose one of three presentation styles **automatically** based on the paragraph content:\r\n   - **Numbered list (1., 2., 3., ...)** — use when the paragraph expresses steps, instructions, an explicit sequence, commands/imperatives, or contains ordering words (first, then, next, afterward, finally, step, etc.).\r\n   - **Bulleted list (• or -)** — use when the paragraph is a short item, single-line fragment, shopping/feature list, or several noun phrases/items.\r\n   - **Plain paragraph** — use when the paragraph is narrative, explanatory, descriptive, or flows as normal prose (multiple sentences, stories, discussion)."
                + "4) Each corrected paragraph must appear on a **new line**, separated by a blank line from the next paragraph."
                + "5) If multiple paragraphs are chosen to be listed (numbered or bulleted), write each item on its own line. Do NOT combine multiple items into a single line."
                + "6) Use numbered lists for sequences when order matters; prefer bullets when order does not matter."
                + "7) Keep formatting minimal and plain-text (do not add Markdown code fences). Use a period after numbers and one space after bullets. Example formats:\r\n   Numbered:\r\n   1. First corrected step.\r\n \r\n   2. Second corrected step.\r\n \r\n   Bulleted:\r\n   • First item.\r\n \r\n   • Second item.\r\n \r\n   Plain paragraph:\r\n   Corrected paragraph text."
                + "8) Do NOT include any explanations, summaries, or extra commentary — output only the \"Corrected Text:\" section and the formatted paragraphs as specified.")*/
                new SystemChatMessage(
                    $"You are a chat expert and knowledgeable SQL query generator. Generate SQL queries based on the schema given below. {schemaJson} " +
                    "Return only SQL Server compatible queries (do not use LIMIT, OFFSET, ARRAY_AGG, JSON_OBJECT, etc.). " +
                    "Do NOT generate any SQL query that could result in the error: 'Operand type clash: uniqueidentifier is incompatible with tinyint'. " +
                    "Avoid joining or comparing columns of different types, especially uniqueidentifier and tinyint. " +
                    "Do not display any real table names in the chat response. Instead, show relevant user-friendly names or descriptions. " +
                    "Avoid getting primary or foreign key id values in queries. Try to get only the text column values. " +
                    "Given a user's natural language request, return the chat reply in the chatresponse property and the SQL query in a JSON object with the property 'sql'. " +
                    "If there is no relevant SQL query creation possible, please reply empty in this property. " +
                    "Return the result in this JSON format {\"chatresponse\":\"\",\"sql\":\"\"}."
                )
            };
            chatHistory.Add(new UserChatMessage(userPrompt));
            var response = await chatClient.CompleteChatAsync(chatHistory);
            var botResponse = response.Value.Content.Last().Text;

            AISqlQueryResponse agentResponse = null;
            try
            {
                string cleanedJson = botResponse.Replace("```json", "").Replace("```", "").Trim();
                agentResponse = JsonSerializer.Deserialize<AISqlQueryResponse>(cleanedJson);
                if (agentResponse != null && !string.IsNullOrWhiteSpace(agentResponse.Sql))
                {
                    return new AISqlQueryResponse
                    {
                        Sql = agentResponse.Sql.Trim(),
                        ChatResponse = agentResponse.ChatResponse
                    };
                }
            }
            catch (Exception)
            {
                return agentResponse;
            }
            return agentResponse;
        }

        public async Task<string> GammerCheckerAgent(string UserPrompt)
        {
            var chatClient = ConnectAzureAI(); // Await the Task to get ChatClient
            var chatHistory = new List<ChatMessage>
            {
                new SystemChatMessage("You are a grammar correction assistant. "+
                            "Provide only the corrected version of the text.")
            };

            chatHistory.Add(new UserChatMessage(UserPrompt));

            var response = await chatClient.CompleteChatAsync(chatHistory);

            var botResponse = response.Value.Content.Last().Text;
            chatHistory.Add(new AssistantChatMessage(botResponse));

            return botResponse;
        }

        public async Task<string> TextStatementAnalyzer(string[] UserPrompt, string industry, string subindustry)
        {
            var chatClient = ConnectAzureAI();
            var chatHistory = new List<ChatMessage>
            {
                new SystemChatMessage($"You are a text descritpion analyzer expert assistant. " +
                            $"You will understand the list of statements for the " +
                            $"given industry{industry} and subindustry{subindustry} and " +
                            $"re-arrange the best top 3 description from the list and " +
                            $"arrange them sequentially. "
                          )



            };

            // Combine the string[] into a single string, or create multiple UserChatMessages if needed.
            string combinedPrompt = string.Join("\n", UserPrompt);
            chatHistory.Add(new UserChatMessage(combinedPrompt));

            var response = await chatClient.CompleteChatAsync(chatHistory);

            var botResponse = response.Value.Content.Last().Text;
            chatHistory.Add(new AssistantChatMessage(botResponse));

            return botResponse;
        }



        public string GenerateSystemPromptForPartnerSolutionSchema()
        {
            return @"You are a highly knowledgeable chat and SQL query generation assistant with expertise in the following database schema for a partner solution platform. 
You understand the structure, relationships, and purposes of these tables. Use only the columns listed for each table. 
Return only SQL Server compatible queries (do not use LIMIT, OFFSET, ARRAY_AGG, JSON_OBJECT, etc.).

Table schemas:
- geo: geoId, locale, geoname, geodescription, DisplayOrder, status, rowChangedBy, rowChangedDate
- Industry: IndustryId, IndustryName, IndustryDescription, Status, RowChangedBy, RowChangedDate, IndustrySlug, image_main, image_mobile
- IndustryResourceLink: IndustryResourceLinkId, industryThemeId, resourceLinkId, title, resourceLink, status, rowChangedBy, rowChangedDate
- IndustryShowcasePartnerSolution: IndustryShowcasePartnerSolutionId, industryThemeId, partnerId, marketplaceLink, status, rowChangedBy, rowChangedDate, PartnerName, websiteLink, overviewDescription, logoFileLink, PartnerNameSlug
- IndustryTargetCustomerProspect: IndustryTargetCustomerProspectId, industryThemeId, targetPersonaTitle, status, rowChangedBy, rowChangedDate
- IndustryTheme: industryThemeId, industryId, subIndustryId, partnerId, Theme, industryThemeDesc, imageFileLink, status, isPublished, rowChangedBy, rowChangedDate, image_thumb, image_main, image_mobile, solutionStatusId, industryThemeSlug
- IndustryThemeBySolutionArea: IndustryThemeBySolutionAreaId, industryThemeId, solutionAreaId, solutionDesc, status, rowChangedBy, rowChangedDate
- organization: orgId, orgName, orgDescription, status, rowChangedBy, rowChangedDate, logoFileLink, orgWebsite, UserType
- partnerSolution: partnerSolutionId, UserId, IndustryId, SubIndustryId, OrganizationId, solutionName, solutionDescription, solutionOrgWebsite, marketplaceLink, specialOfferLink, logoFileLink, SolutionStatusId, IsPublished, rowChangedBy, rowChangedDate, partnerSolutionSlug, IndustryDesignation, ParentsolutionId
- PartnerSolutionAvailableGeo: PartnerSolutionAvailableGeoId, PartnerSolutionId, GeoId, status, rowChangedBy, rowChangedDate
- partnerSolutionByArea: partnerSolutionByAreaId, partnerSolutionId, solutionAreaId, areaSolutionDescription, status, rowChangedBy, rowChangedDate
- partnerSolutionPlay: partnerSolutionPlayId, solutionAreaId, orgId, partnerSolutionPlaySlug, solutionPlayName, solutionPlayDescription, solutionPlayOrgWebsite, marketplaceLink, specialOfferLink, logoFileLink, SolutionStatusId, IsPublished, rowChangedBy, rowChangedDate, image_thumb, image_main, image_mobile, IndustryDesignation
- PartnerSolutionPlayAvailableGeo: PartnerSolutionPlayAvailableGeoId, PartnerSolutionPlayId, GeoId, status, rowChangedBy, rowChangedDate
- partnerSolutionPlayByPlay: partnerSolutionPlayByPlayId, partnerSolutionPlayId, solutionPlayId, playSolutionDescription, status, rowChangedBy, rowChangedDate
- partnerSolutionPlayResourceLink: partnerSolutionPlayResourceLinkId, partnerSolutionPlayByPlayId, resourceLinkId, resourceLinkTitle, resourceLinkUrl, status, resourceLinkOverview, rowChangedBy, rowChangedDate
- partnerSolutionResourceLink: partnerSolutionResourceLinkId, partnerSolutionByAreaId, resourceLinkId, resourceLinkTitle, resourceLinkUrl, status, rowChangedBy, rowChangedDate, resourceLinkOverview, eventDateTime
- resourceLink: resourceLinkId, resourceLinkName, resourceLinkDescription, DisplayOrder, status, rowChangedBy, rowChangedDate
- solutionArea: solutionAreaId, solutionAreaName, solAreaDescription, status, rowChangedBy, rowChangedDate, IsDisplayOnPartnerProfile, DisplayOrder, image_thumb, image_mobile, image_main, solutionareaslug
- solutionPlay: solutionPlayId, solutionAreaId, solutionPlayThemeSlug, solutionPlayName, solutionPlayDesc, solutionPlayLabel, solutionStatusId, rowChangedBy, rowChangedDate, image_thumb, image_main, image_mobile, IsPublished
- SolutionStatus: SolutionStatusId, SolutionStatus, DisplayLabel
- Spotlight: SpotlightId, OrganizationId, UsecaseId, PartnerSolutionId, SpotlightOverview, rowChangedBy, rowChangedDate
- SubIndustry: SubIndustryId, IndustryId, SubIndustryName, SubIndustryDescription, Status, RowChangedBy, RowChangedDate, SubIndustrySlug
- usecaseOrganization: usecaseId, organizationId

You should answer questions, generate queries, or provide insights strictly based on this schema. Always use correct table and column names, and consider relationships between tables. If asked for examples, use realistic sample data matching the schema.";
        }
    }
}