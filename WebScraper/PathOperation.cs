using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class PathOperation
    {
        public static string GetFolderFromDomain(string domain) { 
            Uri uri = new Uri(domain);
            return uri.Host;
        }
    }
}
