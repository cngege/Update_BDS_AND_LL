using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Update_BDS_AND_LL
{
    public class Resources
    {
        public string? name { get; set; }
        public string? updated_at { get; set; }
        public string? browser_download_url { get; set; }


    }

    public class LLJson
    {
        public string? tag_name { get; set; }
        public string? name { get; set; }
        public List<Resources>? assets { get; set; }
        public string? body { get; set; }
    }
}
