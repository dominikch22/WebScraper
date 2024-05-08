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

                // Parse the URL
               

                string localPath = urlFragment.Trim().Trim('\'', '\"', '(').Trim(')', '\"', '\'');
                string finalUrl = MakeUrl(contextUrl, localPath);
                //finalUrl = GetUrlwithoutParams(finalUrl);
                urls.Add(finalUrl);
                /*// Parse the URL
              *//*  Uri uri = new Uri(cleanedUrl);

                // Get the URL without the query string
                cleanedUrl = $"{uri.AbsolutePath}";*//*
                if(cleanedUrl.StartsWith(".."))
                    cleanedUrl = GetOneLowerUrl

                if (cleanedUrl[0] == '/')
                    urls.Add(cleanedUrl);*/
            }

            return urls;
        }


        public static string ChangeUrlToWindowsPath(string url, string domain) {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/")) {
                url += "/index.html";
                uri = new Uri(url);
            }


            string localPath = uri.LocalPath.Replace("/", "\\");

            string currentDirectory = $"C:\\webscraper\\{domain}\\";

            /*if (localPath.Length > 100)
                localPath = localPath.MakeShorterWindowsPath();*/

            string combinedPath = currentDirectory + localPath;

            combinedPath = combinedPath.Replace("\\\\", "\\");

            return combinedPath;
        }

        public static string MakeUrl(string contextUrl, string localPath) {
            contextUrl = GetUrlwithoutParams(contextUrl);
            
            if (localPath.StartsWith("http")) {
                return GetUrlwithoutParams(localPath);
            }
            else if (localPath.StartsWith(".."))
            {
                contextUrl = GetOneLowerUrl(contextUrl);
                string url = contextUrl + localPath.Substring(2);
                return  url;
            }
            else if (localPath[0] == '/') {
                string baseUrl = GetBaseUrl(contextUrl);
                
                return GetUrlwithoutParams(baseUrl + localPath);
            }     
            else {
                return "";
            }
        }


        public static string GetOneLowerUrl(string url) {
            Uri uri = new Uri(url);

            string[] absultePath = uri.AbsolutePath.Split('/');
            string loweredAbsolutePath = "";

            for (int i = 0; i < absultePath.Length - 1; i++) {
                loweredAbsolutePath += "/" + absultePath[i];
            }

            return $"{uri.Scheme}://{uri.Host}{loweredAbsolutePath.Substring(1)}"; 

        }

        public static string GetUrlwithoutParams(string url) {
            Uri uri = new Uri(url);

            // Get the URL without the query string
            string baseUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";

            return baseUrl;
        }
        public static string GetBaseUrl(string url)
        {

            Uri uri = new Uri(url);

            // Get the URL without the query string
            string baseUrl = $"{uri.Scheme}://{uri.Host}";

            return baseUrl;
        }

    }
    
}
