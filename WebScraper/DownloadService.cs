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
            }
        }
       
        public void StopDownloading()
        {
            foreach (WebScrapper scrapper in WebScrappers)
            {
                if(scrapper.Client != null)
                    scrapper.Client.CancelAsync();
            }
        }

      



    }
}
