using Knack.API.Interfaces;
using System.Data;
using System.Text;

namespace Knack.API.Builders
{
    public class ReportBuilder : IReportBuilder
    {
        private readonly IReportManager _reportManager;

        public ReportBuilder(IReportManager reportManager)
        {
            _reportManager = reportManager;
        }
        public async Task<string> GetTextReport()
        {
            var textBuilder = new StringBuilder();
            var dataSet = _reportManager.GetReportData();

            foreach (var table1 in dataSet.Tables)
            {
                foreach (var row in (table1 as DataTable).Rows)
                {

                    for (int i = 0; i < (row as DataRow).Table.Columns.Count; i++)
                    {
                        textBuilder.AppendLine(((System.Data.DataRow)row)[i].ToString());
                    }
                }

            }

            return textBuilder.ToString();
        }
    }
}
