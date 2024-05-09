using EmbedIO;
using EmbedIO.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class LocalHttpServer
    {
        public static async Task Start(string folder)
        {
            var url = "http://localhost:8080/";
            var webRoot = $"C:\\webscraper\\{folder}";

            using (var server = new WebServer(url))
            {
                /*server.RegisterModule(new StaticFilesModule(".", useDirectoryBrowser: true)
                {
                    DefaultExtension = ".html", // Set default extension to .html
                    UseRamCache = true, // Use cache for better performance
                    HideExtensions = true // Hide extensions in URLs
                });*/

                server.WithStaticFolder("/", webRoot, true, m => m.WithDefaultExtension(".html"));
                server.WithLocalSessionManager();
                await server.RunAsync();
            }
        }
    }
}
