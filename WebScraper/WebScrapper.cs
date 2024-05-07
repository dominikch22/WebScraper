using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebScraper
{
    public class WebScrapper
    {
        public MainBinding MainBinding;
        //public List<HtmlContent> HtmlContents { get; set; }
        public HtmlNodeCollection HtmlNodes { get; set; }
        public string Content;
        public string Url;

        public WebScrapper(MainBinding mainBinding, string url)
        {
            MainBinding = mainBinding;
            Url = url;
            //HtmlContents = new List<HtmlContent>();
            //Html
        }
        public async Task StartDownloading()
        {
            //List<Task> downloadTasks = new List<Task>();
            await DownloadHtml(Url);

           /* foreach (string url in Urls)
            {
                //ThreadPool.QueueUserWorkItem((async (_) => await ));
                //DownloadHtml(url);
                downloadTasks.Add(DownloadHtml(url));
            }*/

            //await Task.WhenAll(downloadTasks);

            IndexResources();
            StartDownloadingResources();


        }

        public async void StartDownloadingResources()
        {

           /* foreach (HtmlContent htmlContent in HtmlContents)
            {
                IndexResources(htmlContent);
                //DownloadResourcesFromNodes(nodes);
            }*/

            foreach (FileBinding file in MainBinding.FileBindings) {
                DownloadResource(file);
            }
            /*bool allTasksCompleted = CountdownEvent.Wait(TimeSpan.FromSeconds(1));
            if (allTasksCompleted)
            {
               
            }*/
        }

        public async Task DownloadHtml(string url)
        {
            using (HttpClient client = new HttpClient())
            {

                /*client.DownloadProgressChanged += (sender, e) =>
                {
                    MainBinding.TotalProgressBar = e.ProgressPercentage;
                };*/

                /*client.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        Console.WriteLine($"Error downloading file: {e.Error.Message}");
                    }
                    else
                    {
                        Console.WriteLine("File downloaded successfully.");
                    }
                };*/

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // Pobierz zawartość odpowiedzi jako ciąg znaków (HTML)
                Content = await response.Content.ReadAsStringAsync();
                //Content = Encoding.UTF8.GetString(await client.get(new Uri(url)));
                //HtmlContents.Add(content);

                MainBinding.FileBindings.Add(new FileBinding(url, response.Content.Headers.ContentLength.Value, 100));

                HtmlNodes = await GetImgLinkScriptNodesFromHtml(Content);


                //HtmlNodes.Add(nodes);
                /*HtmlContent htmlContent = new HtmlContent();
                htmlContent.Url = url;
                htmlContent.Nodes = nodes;

                HtmlContents.Add(htmlContent);*/

            }
        }

        public void IndexResources()
        {
            string resourceUrl = null;
            foreach (HtmlNode node in HtmlNodes)
            {
                if (node.Name == "img" || node.Name == "script")
                {
                    resourceUrl = node.GetAttributeValue("src", null);
                }
                else if (node.Name == "link")
                {
                    resourceUrl = node.GetAttributeValue("href", null);
                }

                if (!string.IsNullOrEmpty(resourceUrl))
                {
                    FileBinding fileBinding = new FileBinding();
                    fileBinding.Url = Url + resourceUrl;

                    //Uri uri = new Uri(resourceUrl);
                    //fileBinding.FileName = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileName(uri.LocalPath));
                    fileBinding.FileName = resourceUrl;


                    fileBinding.Downloading = 0;

                    if (!MainBinding.FileBindings.Contains(fileBinding))
                        MainBinding.FileBindings.Add(fileBinding);

                    //DownloadResource(resourceUrl);
                }
            }


        }


        public async Task<HtmlNodeCollection> GetImgLinkScriptNodesFromHtml(string htmlContent)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//img|//link[@rel='stylesheet']|//script");
            return nodes;
        }

        public async Task DownloadResourcesFromNodes(HtmlNodeCollection nodes)
        {
            string resourceUrl = null;
            foreach (HtmlNode node in nodes)
            {
                if (node.Name == "img" || node.Name == "script")
                {
                    resourceUrl = node.GetAttributeValue("src", null);
                }
                else if (node.Name == "link")
                {
                    resourceUrl = node.GetAttributeValue("href", null);
                }
            }

            if (!string.IsNullOrEmpty(resourceUrl))
            {
                //DownloadResource(resourceUrl);
            }
        }

        public async Task DownloadResource(FileBinding file)
        {
            try
            {
                using (WebClient client = new WebClient())
                {

                    client.DownloadProgressChanged += (sender, e) =>
                    {
                        file.Downloading = e.ProgressPercentage;
                        file.Size = e.TotalBytesToReceive;
                    };

                    if (file.Url == null)
                        return;
                    

                    Uri uri = new Uri(file.Url);

                    string localPath = uri.LocalPath.Replace("/", "\\");
                    string currentDirectory = @"C:\webscraper\";


                    if (MainBinding.ShorterDirectories && localPath.Length > 100)
                        localPath = localPath.MakeShorterPath();
                    string combinedPath = currentDirectory + localPath;

                    combinedPath = combinedPath.Replace("\\\\", "\\");

                    

                    Directory.CreateDirectory(Path.GetDirectoryName(combinedPath));


                    await client.DownloadFileTaskAsync(file.Url, combinedPath);

                    MainBinding.DownloadSuccess += 1;
                    CalculateTotalProgress();
                }
            }
            catch(Exception e) {
                MainBinding.DownloadFailure += 1;

                file.Error = e.Message;
            }
            
        }


        public void CalculateTotalProgress() { 
            double count = MainBinding.FileBindings.Count;
            double sum = 0;
            foreach(var file in MainBinding.FileBindings) {
                if (file.Error != null)
                    sum += 1;
                else
                    sum += (double)file.Downloading / 100;
            }
            MainBinding.TotalProgressBar = (int)(count / sum)*100;
        }
    }
}
