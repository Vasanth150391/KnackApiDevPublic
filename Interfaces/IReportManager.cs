using Microsoft.Data.SqlClient;
using System.Data;

namespace Knack.API.Interfaces
{
    public interface IReportManager
    {
        public DataSet GetReportData();
    }
}
