using System.Threading.Tasks;
using Knack.API.Interfaces;
using Knack.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Collections.Generic;

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

                throw ex;
            }
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
