using System.Threading.Tasks;
using Knack.API.Interfaces;
using Knack.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Knack.API.DataManagers
{
    public class SqlQueryManager : ISqlQueryManager
    {
        private readonly KnackContext _context;
        public SqlQueryManager(KnackContext context)
        {
            _context = context;
        }
        public async Task<object> ExecuteQueryAsync(string sqlQuery)
        {
            try
            {
                if (!IsSafeReadOnlySqlQuery(sqlQuery))                
                    throw new InvalidOperationException("Only single-statement read-only parameterized SELECT queries are allowed.");
                if (Regex.IsMatch(sqlQuery, @"'([^']|'')*'"))
                    throw new InvalidOperationException("SQL string literals are not allowed. Use parameters for all dynamic values.");

                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;

                if (command.Connection.State != ConnectionState.Open)
                     await command.Connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                var results = new List<Dictionary<string, object>>();
                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                         row[reader.GetName(i)] = reader.GetValue(i);
                    }
                    results.Add(row);
                }
                return results;                                    

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private static bool IsSafeReadOnlySqlQuery(string sqlQuery)
        {
            if (string.IsNullOrWhiteSpace(sqlQuery))
                return false;

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
            // Reject inline string literals to prevent raw user-controlled SQL text execution.
            if (Regex.IsMatch(normalized, @"'([^']|'')*'"))
                return false;

            // Require at least one SQL parameter placeholder (e.g., @p0) to enforce parameterized shape.
            if (!Regex.IsMatch(normalized, @"@\w+"))
                return false;
            return true;
        }
        public async Task<string> GetSchemaAsync()
        {
            try
            {
                var results = new List<string>();
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "SELECT TABLE_NAME, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS ORDER BY TABLE_NAME, ORDINAL_POSITION";
                command.CommandType = CommandType.Text;

                if (command.Connection.State != ConnectionState.Open)
                    await command.Connection.OpenAsync();

                using var reader = await command.ExecuteReaderAsync();
                string currentTable = null;
                var columns = new List<string>();
                while (await reader.ReadAsync())
                {
                    var table = reader.GetString(0);
                    var column = reader.GetString(1);
                    if (currentTable != table)
                    {
                        if (currentTable != null)
                            results.Add($"{currentTable}: {string.Join(", ", columns)}");
                        currentTable = table;
                        columns = new List<string>();
                    }
                    columns.Add(column);
                }
                if (currentTable != null)
                    results.Add($"{currentTable}: {string.Join(", ", columns)}");
                return string.Join("\n", results);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
