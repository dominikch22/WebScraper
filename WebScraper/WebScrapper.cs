using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebScraper
{
    public class WebScrapper
    {
        public MainBinding MainBinding;
        public List<HtmlContent> HtmlContents { get; set; }
        //public List<HtmlNodeCollection> HtmlNodes { get; set; }
        public List<string> Urls;
        public CountdownEvent CountdownEvent;

        public WebScrapper(MainBinding mainBinding, List<string> urls)
        {
            MainBinding = mainBinding;
            Urls = urls;
            CountdownEvent = new CountdownEvent(Urls.Count);
            HtmlContents = new List<HtmlContent>();
            //HtmlNodes = new List<HtmlNodeCollection>();
        }
        public async Task StartDownloading()
        {
            List<Task> downloadTasks = new List<Task>();
            foreach (string url in Urls)
            {
                //ThreadPool.QueueUserWorkItem((async (_) => await ));
                //DownloadHtml(url);
                downloadTasks.Add(DownloadHtml(url));
            }

            await Task.WhenAll(downloadTasks);

            StartDownloadingResources();


        }

        public void StartDownloadingResources()
        {

            foreach (HtmlContent htmlContent in HtmlContents)
            {
                IndexResources(htmlContent);
                //DownloadResourcesFromNodes(nodes);
            }

            /*bool allTasksCompleted = CountdownEvent.Wait(TimeSpan.FromSeconds(1));
            if (allTasksCompleted)
            {
               
            }*/
        }

        public async Task DownloadHtml(string url)
        {
            using (WebClient client = new WebClient())
            {

                client.DownloadProgressChanged += (sender, e) =>
                {
                    MainBinding.TotalProgressBar = e.ProgressPercentage;
                };

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


                string content = Encoding.UTF8.GetString(await client.DownloadDataTaskAsync(new Uri(url)));
                //HtmlContents.Add(content);

                HtmlNodeCollection nodes = await GetImgLinkScriptNodesFromHtml(content);
                //HtmlNodes.Add(nodes);
                HtmlContent htmlContent = new HtmlContent();
                htmlContent.Url = url;
                htmlContent.Nodes = nodes;

                HtmlContents.Add(htmlContent);

                CountdownEvent.Signal();
            }
        }

        public void IndexResources(HtmlContent htmlContent)
        {
            string resourceUrl = null;
            foreach (HtmlNode node in htmlContent.Nodes)
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
                    fileBinding.Url = htmlContent.Url + resourceUrl;

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

        public void DownloadResource(string resourceUrl)
        {
            using (WebClient client = new WebClient())
            {

                client.DownloadProgressChanged += (sender, e) =>
                {
                    MainBinding.TotalProgressBar = e.ProgressPercentage;
                };


            }
        }

    }
}
