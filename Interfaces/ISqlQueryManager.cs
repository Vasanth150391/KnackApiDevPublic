using System.Threading.Tasks;

namespace Knack.API.Interfaces
{
    public interface ISqlQueryManager
    {
        Task<object> ExecuteQueryAsync(string sqlQuery);
        Task<string> GetSchemaAsync();
    }
}
