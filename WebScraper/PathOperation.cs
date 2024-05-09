﻿using System;
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

        public static string ChangeUrlToWindowsPath(string url, string folder)
        {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.AbsolutePath) || uri.AbsolutePath.Equals("/"))
            {
                url += "/index.html";
            }

            if (url.EndsWith(".php"))
            {
                url = url.Substring(0, url.Length - 3);
                url = url + "html";
            }

            if (!uri.AbsolutePath.Contains("."))
            {
                url = url + ".html";
            }

            uri = new Uri(url);


            string localPath = uri.LocalPath.Replace("/", "\\");

            string currentDirectory = $"C:\\webscraper\\{folder}\\";

            /*if (localPath.Length > 100)
                localPath = localPath.MakeShorterWindowsPath();*/

            string combinedPath = currentDirectory + localPath;

            combinedPath = combinedPath.Replace("\\\\", "\\");

            return combinedPath;
        }

        public static string MakeUrl(string contextUrl, string localPath)
        {
            //contextUrl = GetUrlwithoutParams(contextUrl);

            if (localPath.StartsWith("http"))
            {
                return localPath;
                //return GetUrlwithoutParams(localPath);
            }
            else if (localPath.StartsWith(".."))
            {
                contextUrl = GetOneLowerUrl(contextUrl);
                string url = contextUrl + localPath.Substring(2);
                return url;
            }
            else if (localPath[0] == '/')
            {
                string baseUrl = GetBaseUrl(contextUrl);

                return baseUrl + localPath;
            }
            else
            {
                return "";
            }
        }


        public static string GetOneLowerUrl(string url)
        {
            Uri uri = new Uri(url);

            string[] absultePath = uri.AbsolutePath.Split('/');
            string loweredAbsolutePath = "";

            for (int i = 0; i < absultePath.Length - 2; i++)
            {
                loweredAbsolutePath += "/" + absultePath[i];
            }

            return $"{uri.Scheme}://{uri.Host}{loweredAbsolutePath.Substring(1)}";

        }

        public static string GetUrlwithoutParams(string url)
        {
            Uri uri = new Uri(url);

            string baseUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";

            return baseUrl;
        }
        public static string GetBaseUrl(string url)
        {

            Uri uri = new Uri(url);

            string baseUrl = $"{uri.Scheme}://{uri.Host}";

            return baseUrl;
        }

        public static string MakeShorterWindowsPath(string path)
        {
            string[] directories = path.Split('\\');


            for (int i = 0; i < directories.Length - 1; i++)
            {
                if (directories[i].Length > 36)
                    directories[i] = GenerateRandomDirectoryName(7);
            }

            string shortenedPath = string.Join("\\", directories);
            return shortenedPath;
        }

        public static string MakeShorterLocalPath(string path)
        {
            string[] directories = path.Split('/');


            for (int i = 0; i < directories.Length - 1; i++)
            {
                if (directories[i].Length > 25)
                    directories[i] = GenerateRandomDirectoryName(7);
            }

            string shortenedPath = string.Join("/", directories);
            return shortenedPath;
        }


        static string GenerateRandomDirectoryName(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder sb = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            return sb.ToString();
        }
    }
}
