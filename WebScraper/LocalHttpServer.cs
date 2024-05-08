using EmbedIO;
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
                server.WithStaticFolder("/", webRoot, true);

                await server.RunAsync();
            }
        }
    }
}
