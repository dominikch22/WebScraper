using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class DownloadService
    {
        public MainBinding MainBinding { get; set; }
        public string[] Urls;
        public string Domain;
        public string Url;
        public List<WebClient> WebClients;
        public Dictionary<string, string> ChangedUrls;

        public List<WebScrapper> WebScrappers;

        public DownloadService(MainBinding mainBinding) { 
            MainBinding = mainBinding;
            Domain = (string)mainBinding.Domain.Clone();
            GetUrls();
            WebClients = new List<WebClient>();
            ChangedUrls = new Dictionary<string, string>();
            WebScrappers = new List<WebScrapper>();
        }

        public void GetUrls()
        {
            string urls = MainBinding.Urls.Replace(" ", "").Replace("\t", "").Trim();
            Urls = urls.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Url = Urls[0];
        }

        public async Task Start() {
            GetUrls();
            List<Task> tasks = new List<Task>();
            foreach (string url in Urls)
            {
                WebScrapper webScrapper = new WebScrapper(MainBinding, url, Domain);
                WebScrappers.Add(webScrapper);
                tasks.Add(webScrapper.IndexAndDownload());
            }
            Task.WhenAll(tasks)
                .ContinueWith(_ => { DownloadHtmlResources(); });
            
        }

        public async Task DownloadHtmlResources()
        {
            List<Task> downloadResourceTask = new List<Task>();
            foreach (FileBinding file in MainBinding.FileBindings)
            {
                WebScrapper webScrapper = new WebScrapper(MainBinding, file.Url, file.Domain);
                WebScrappers.Add(webScrapper);
                downloadResourceTask.Add(webScrapper.DownloadResource(file));
                //await Task.Delay(50);

            }
            Task.WhenAll(downloadResourceTask)
                .ContinueWith(_ => { DownloadCssResources(); });
            
        }

        public async Task DownloadCssResources() {
            foreach (FileBinding file in MainBinding.FileBindings)
            {
                WebScrapper webScrapper = new WebScrapper(MainBinding, file.Url, file.Domain);
                WebScrappers.Add(webScrapper);
                webScrapper.DownloadResource(file);
                //await Task.Delay(50);

            }
        }
        /* public async Task Start() {
             await IndexAndDownloadAllUrls();
             DownloadAllResources();
         }
         public async Task IndexAndDownloadUrl() {
             string content = await DownloadHtml(url);
             IndexResources(content, url);

             *//*foreach (string url in Urls) {
                 string content = await DownloadHtml(url);
                 IndexResources(content, url);
                 *//*WebScrapper web = new WebScrapper(MainBinding, url, MainBinding.Domain);
                 await web.DownloadAndIndexReosurces();*//*
             }*//*
         }*/

        /*public async Task<string> DownloadHtml(string url)
        {
            FileBinding file = new FileBinding(url, 0, 100, Domain);
            if (!FileBindingsContains(file))
                MainBinding.FileBindings.Add(file);
            else
                Console.WriteLine("");

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    file.Size = response.Content.Headers.ContentLength.Value;

                    Uri uri = new Uri(file.Url);

                    string localPath = uri.LocalPath.Replace("/", "\\");
                    if (localPath.Equals("\\"))
                        localPath = "\\index.html";

                    string currentDirectory = $"C:\\webscraper\\{file.Domain}\\";

                    if (MainBinding.ServerPaths && localPath.Length > 100)
                        localPath = localPath.MakeShorterWindowsPath();

                    string combinedPath = currentDirectory + localPath;

                    combinedPath = combinedPath.Replace("\\\\", "\\");

                    if (MainBinding.ServerPaths)
                        file.Path = $"http://{file.Domain}" + localPath;
                    else
                        file.Path = combinedPath;

                    Directory.CreateDirectory(Path.GetDirectoryName(combinedPath));

                    File.WriteAllText(combinedPath, content);

                    return content;
                }
            }
            catch (Exception ex)
            {
                file.Error = ex.Message;
                return "";
            }
        }*/

        /*public void IndexResources(string content, string contextUrl)
        {
            // Parse the URL
            //Uri domain = new Uri(contextUrl);

            // Get the URL without the query string
            //contextUrl = $"{domain.Scheme}://{domain.Host}";



            HtmlNodeCollection htmlNodes = GetImgLinkScriptNodesFromHtml(content);


            string localPath = null;
            foreach (HtmlNode node in htmlNodes)
            {
                if (node.Name == "img" || node.Name == "script")
                {
                    localPath = node.GetAttributeValue("src", null);
                    string shorterLocalPath = localPath.MakeShorterWindowsPath();
                    node.SetAttributeValue("src", shorterLocalPath);
                }
                else if (node.Name == "link")
                {
                    localPath = node.GetAttributeValue("href", null);
                    string shorterLocalPath = localPath.MakeShorterWindowsPath();
                    node.SetAttributeValue("link", shorterLocalPath);

                }

                if (!string.IsNullOrEmpty(localPath))
                {
                    FileBinding fileBinding = new FileBinding();

                    string shorterLocalPath = localPath.MakeShorterWindowsPath();

                    string url = CssParser.MakeUrl(contextUrl, localPath);
                    string shorterLocation = CssParser.ChangeUrlToWindowsPath(url, );

                    fileBinding.Url = url;

                    fileBinding.FileLocation = localPath;
                    fileBinding.Domain = Domain;

                    fileBinding.Downloading = 0;

                    if (!MainBinding.FileBindings.Contains(fileBinding))
                        MainBinding.FileBindings.Add(fileBinding);
                }
            }
        }*/

        /*public HtmlNodeCollection GetImgLinkScriptNodesFromHtml(string htmlContent)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//img|//link[@rel='stylesheet']|//script");
            return nodes;
        }

        public async Task DownloadAllResources()
        {
            WebClients.Clear();
            foreach (FileBinding file in MainBinding.FileBindings)
            {
                DownloadResource(file);
            }


        }

        public async Task DownloadResource(FileBinding file)
        {
            if (file.Downloading == 100 || file.Error != null)
                return ;
            try
            {
                using (WebClient client = new WebClient())
                {
                    WebClients.Add(client);
                    client.DownloadProgressChanged += (sender, e) =>
                    {
                        file.Downloading = e.ProgressPercentage;
                        file.Size = e.TotalBytesToReceive;
                    };

                    if (file.Url == null)
                        return ;

                    
                    string path = CssParser.ChangeUrlToWindowsPath(file.Url, file.Domain);

                    Directory.CreateDirectory(Path.GetDirectoryName(path));

                    if (path.EndsWith(".css") || path.EndsWith(".min"))
                    {
                        

                        string cssContent = await client.DownloadStringTaskAsync(file.Url);
                        await IndexCssContent(cssContent, file.Url);
                        File.WriteAllText(path, cssContent);
                    }
                    else
                    {
                        await client.DownloadFileTaskAsync(file.Url, path);

                    }

                    *//*string content = await client.DownloadStringTaskAsync(file.Url);
                    File.WriteAllText(combinedPath, content);*//*


                    MainBinding.DownloadSuccess += 1;
                    CalculateTotalProgress();

                }
            }
            catch (Exception e)
            {
                MainBinding.DownloadFailure += 1;

                file.Error = e.Message;
            }
        }

       
        public async Task IndexCssContent(string cssContent, string contextUrl)
        {
            List<string> resourceUrls = CssParser.getCssUrls(cssContent, contextUrl);

            foreach (string localPath in resourceUrls) {
                if (!string.IsNullOrEmpty(localPath))
                {
                    FileBinding fileBinding = new FileBinding();

                    // Parse the URL
                    string url = CssParser.MakeUrl(contextUrl, localPath);
                    //Uri uri = new Uri("https://akademiabialska.pl" + resourceUrl);

                    // Get the URL without the query string
                    //string cleanedUrl = $"{uri.Scheme}://{uri.Host}{uri.AbsolutePath}";
                    fileBinding.Url = url;



                    fileBinding.FileLocation = localPath;
                    fileBinding.Domain = Domain;

                    fileBinding.Downloading = 0;

                    if (!FileBindingsContains(fileBinding)) {
                        MainBinding.FileBindings.Add(fileBinding);
                        await DownloadResource(fileBinding);
                    }
                    else
                        Console.WriteLine("");
                }
            }
                
        }*/

        /*public async Task DownloadCssResources(string cssContent, string url) {
            List<string> resourceUrls = CssParser.getCssUrls(cssContent);

            foreach (string resourceUrl in resourceUrls) {
                DownloadResource();

            }

        }*/

        /*public bool FileBindingsContains(FileBinding element)
        {
            foreach (FileBinding file in MainBinding.FileBindings)
            {
                if (element.Url.Equals(file.Url))
                    return true;
            }
            return false;
        }*/
        public void StopDownloading()
        {
            foreach (WebScrapper scrapper in WebScrappers)
            {
                if(scrapper.Client != null)
                    scrapper.Client.CancelAsync();
            }
        }

        public void CalculateTotalProgress()
        {
            double count = MainBinding.FileBindings.Count;
            double sum = 0;
            foreach (var file in MainBinding.FileBindings)
            {
                if (file.Error != null)
                    sum += 1;
                else
                    sum += (double)file.Downloading / 100;
            }
            MainBinding.TotalProgressBar = (int)(count / sum) * 100;
        }



    }
}
