using Knack.API.Models;
using System.Threading.Tasks;

namespace Knack.API.Interfaces
{
    public interface ISqlQueryBuilder
    {
        Task<object> ExecuteQueryAsync(string sqlQuery);
        Task<AISqlQueryResponse> GenerateAISqlQueryAsync(string sqlQuery); // Add this method to match usage
    }
}
