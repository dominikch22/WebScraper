using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WebScraper
{
    public static class StringExtension
    {
        public static string MakeShorterWindowsPath(this string path) {
            string[] directories = path.Split('\\');


            for (int i = 0; i < directories.Length-1; i++) {
                if (directories[i].Length > 36)
                    directories[i] = GenerateRandomDirectoryName(7);
            }

            string shortenedPath = string.Join("\\", directories);
            return shortenedPath;
        }

        public static string MakeShorterLocalPath(this string path) {
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
