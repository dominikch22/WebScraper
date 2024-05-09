using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebScraper
{
    public class CssParser
    {
        public static List<string> getCssUrls(string cssContent, string contextUrl) {
            List<string> urls = new List<string>();

            string pattern = @"url\(([^)]+)\)";

            MatchCollection matches = Regex.Matches(cssContent, pattern);

            foreach (Match match in matches)
            {
                string urlFragment = match.Groups[1].Value;

               

                string localPath = urlFragment.Trim().Trim('\'', '\"', '(').Trim(')', '\"', '\'');
                string finalUrl = PathOperation.MakeUrl(contextUrl, localPath);
                urls.Add(finalUrl);
               
            }

            return urls;
        }



        

    }
    
}
