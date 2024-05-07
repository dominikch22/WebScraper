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
        public List<DownloadResourceTask> DownloadResourceTasks;
        public MainBinding MainBinding { get; set; }
        public string[] Urls;
        public string Domain;

        public DownloadService(MainBinding mainBinding) { 
            MainBinding = mainBinding;
            Domain = (string)mainBinding.Domain.Clone();
            GetUrls();
            DownloadResourceTasks = new List<DownloadResourceTask>();
        }

        public void GetUrls()
        {
            string urls = MainBinding.Urls.Replace(" ", "").Replace("\t", "").Trim();
            Urls = urls.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task Start() {
            await IndexAndDownloadAllUrls();
            DownloadAllResources();
        }
        public async Task IndexAndDownloadAllUrls() {
            foreach(string url in Urls) {
                string content = await DownloadHtml(url);
                IndexResources(content, url);
                /*WebScrapper web = new WebScrapper(MainBinding, url, MainBinding.Domain);
                await web.DownloadAndIndexReosurces();*/
            }
        }

        public async Task<string> DownloadHtml(string url)
        {
            FileBinding file = new FileBinding(url, 0, 100, Domain);
            MainBinding.FileBindings.Add(file);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string content = await response.Content.ReadAsStringAsync();
                    file.Size = response.Content.Headers.ContentLength.Value;

                    return content;
                }
            }
            catch (Exception ex)
            {
                file.Error = ex.Message;
                return "";
            }
        }

        public void IndexResources(string content, string url)
        {
            HtmlNodeCollection htmlNodes = GetImgLinkScriptNodesFromHtml(content);

            string resourceUrl = null;
            foreach (HtmlNode node in htmlNodes)
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
                    fileBinding.Url = url + resourceUrl;

                    fileBinding.FileName = resourceUrl;

                    fileBinding.Downloading = 0;

                    if (!MainBinding.FileBindings.Contains(fileBinding))
                        MainBinding.FileBindings.Add(fileBinding);
                }
            }
        }

        public HtmlNodeCollection GetImgLinkScriptNodesFromHtml(string htmlContent)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlContent);

            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//img|//link[@rel='stylesheet']|//script");
            return nodes;
        }

        public async Task DownloadAllResources()
        {
            foreach (FileBinding file in MainBinding.FileBindings)
            {
                DownloadResource(file);
            }
        }

        public async Task DownloadResource(FileBinding file)
        {
            if (file.Downloading == 100 || file.Error != null)
                return;
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

                    string currentDirectory = $"C:\\webscraper\\{file.Domain}\\";

                    if (MainBinding.ServerPaths && localPath.Length > 100)
                        localPath = localPath.MakeShorterPath();

                    string combinedPath = currentDirectory + localPath;

                    combinedPath = combinedPath.Replace("\\\\", "\\");

                    if (MainBinding.ServerPaths)
                        file.Path = $"http://{file.Domain}" + localPath;
                    else
                        file.Path = combinedPath;

                    Directory.CreateDirectory(Path.GetDirectoryName(combinedPath));

                    await client.DownloadFileTaskAsync(file.Url, combinedPath);

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
