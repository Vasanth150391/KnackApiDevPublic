using Knack.API.Data;
using Knack.API.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Knack.API.DataManagers
{
    public class ReportManager:IReportManager
    {
        private readonly KnackContext _context;        

        public ReportManager(KnackContext context)
        {
            _context = context;
        }

        public DataSet GetReportData()
        {
            var con = new SqlConnection(this._context.Database.GetConnectionString());
            using (var command = new SqlCommand("copy_report", con))
            {
                SqlDataAdapter adapt = new SqlDataAdapter(command);
                command.CommandType = CommandType.StoredProcedure;
                var dataSet=new DataSet();
                adapt.Fill(dataSet);
                return dataSet;
            }
        }
    }
}
