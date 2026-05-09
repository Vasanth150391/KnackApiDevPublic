using Knack.DBEntities;
using Newtonsoft.Json.Linq;

namespace Knack.API.Models
{
    public partial class SFTPGoModel
    {
        public String? username { get; set; }
        public string? password { get; set; }
        //public string? home_dir { get; set; }
        public JObject? permissions { get; set; }
        public int? status { get; set; }
        public FileSystem? filesystem { get; set; }
    }
    public partial class FileSystem
    {
        public int? provider { get; set; }
        public AzBlobConfig azblobconfig { get; set; }
        

    }
    public partial class AzBlobConfig
    {
        public string? container { get; set; }
        public string? account_name { get; set; }
        public AccountKey? account_key { get; set; }
        public string? key_prefix { get; set; }

    }
    public partial class AccountKey
    {
        public string? status { get; set; }
        public string? payload { get; set; }

    }
}
