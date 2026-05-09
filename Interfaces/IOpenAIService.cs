namespace AzureOpenAIWebApi.Services;

public interface IOpenAIService
{
    Task<string> GetChatCompletionAsync(string prompt);
}