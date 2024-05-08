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
        public static List<string> getCssUrls(string cssContent) {
            List<string> urls = new List<string>();

            string pattern = @"url\(([^)]+)\)";

            MatchCollection matches = Regex.Matches(cssContent, pattern);

            foreach (Match match in matches)
            {
                string urlFragment = match.Groups[1].Value;

                string cleanedUrl = urlFragment.Trim().Trim('\'', '\"', '(').Trim(')', '\"', '\'');

                urls.Add(cleanedUrl);
            }

            return urls;
        }





    }
    
}
