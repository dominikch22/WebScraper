﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace WebScraper
{
    public class WebScrapper
    {
        public MainBinding MainBinding;
        public HtmlNodeCollection HtmlNodes { get; set; }
        public string Content;
        public string Url;
        public string Domain;
        public WebClient Client;
        public Dictionary<string, string> ChangedUrls;


        public WebScrapper(MainBinding mainBinding, string url, string domain)
        {
            MainBinding = mainBinding;
            Url = url;
            Domain = domain;
        }

        public async Task IndexAndDownload()
        {
            string content = await DownloadHtml();
            IndexResources(content);

            
        }

        public async Task<string> DownloadHtml()
        {
            FileBinding file = new FileBinding(Url, 0, 100, Domain);
            lock (MainBinding._locker)
            {
                if (!MainBinding.FileBindings.Contains(file))
                    MainBinding.FileBindings.Add(file);
                else
                    Console.WriteLine("");
            }
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(Url);
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

        public void ReplaceHyperLinks(HtmlDocument htmlDocument) {
            var nodes = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    string url = node.GetAttributeValue("href", null);
                    if (url.StartsWith("http")) {
                        Uri uri = new Uri(url);
                        node.SetAttributeValue("href", Domain + uri.AbsolutePath);
                    }
                    
                }
            }
        }

        public void IndexResources(string content)
        {

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            ReplaceHyperLinks(htmlDocument);

            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//img|//link|//script");


            string localPath = null;
            string atrributeName = "";
            foreach (HtmlNode node in nodes)
            {
                if (node.Name == "img" || node.Name == "script")
                {
                    localPath = node.GetAttributeValue("src", null);
                    atrributeName = "src";
                    
                }
                else if (node.Name == "link")
                {
                    localPath = node.GetAttributeValue("href", null);
                    atrributeName = "href";

                }


                if (!string.IsNullOrEmpty(localPath))
                {
                    FileBinding fileBinding = new FileBinding();
                    string url = PathOperation.MakeUrl(Url, localPath);

                    string shorterLocalPath = PathOperation.MakeShorterLocalPath(localPath);
                    node.SetAttributeValue(atrributeName, shorterLocalPath);

                    string windowsPath = PathOperation.ChangeUrlToWindowsPath(url, PathOperation.GetFolderFromDomain(Domain));
                    string shorterWindowsPath = PathOperation.MakeShorterWindowsPath(windowsPath);


                    fileBinding.Url = url;
                    fileBinding.FileLocation = shorterWindowsPath;
                    fileBinding.Domain = Domain;
                    fileBinding.Downloading = 0;

                    lock (MainBinding._locker)
                    {
                        if (!MainBinding.FileBindings.Contains(fileBinding))
                        {
                            MainBinding.FileBindings.Add(fileBinding);
                        }
                    }
                }
            }
            lock (MainBinding._locker)
            {
                MainBinding.FilesCount = MainBinding.FileBindings.Count;

            }
            string windowspHtlmPath = PathOperation.ChangeUrlToWindowsPath(Url, PathOperation.GetFolderFromDomain(Domain));

            Directory.CreateDirectory(Path.GetDirectoryName(windowspHtlmPath));
            htmlDocument.Save(windowspHtlmPath);
        }

      
        public async Task DownloadResource(FileBinding file)
        {
            lock (MainBinding._locker)
            {
                file.Error = null;
                if (file.Downloading == 100 && file.Size != -1)
                    return;
            }
            try
            {
                using (Client = new WebClient())
                {
                    Client.DownloadProgressChanged += (sender, e) =>
                    {
                        file.Downloading = e.ProgressPercentage;
                        file.Size = e.TotalBytesToReceive;
                        
                    };

                    Client.DownloadStringCompleted += (sender, e) =>
                    {
                        file.Downloading = 100;
                    };

                    string path = PathOperation.ChangeUrlToWindowsPath(file.Url, PathOperation.GetFolderFromDomain(file.Domain));

                    Directory.CreateDirectory(Path.GetDirectoryName(file.FileLocation));

                    if (path.EndsWith(".css") || path.EndsWith(".min"))
                    {
                        string cssContent = await Client.DownloadStringTaskAsync(file.Url);
                        await IndexCssContent(cssContent, file.Url);
                        File.WriteAllText(file.FileLocation, cssContent);
                    }
                    else
                    {
                        await Client.DownloadFileTaskAsync(file.Url, file.FileLocation);

                    }


                    CalculateTotalProgress();

                }
            }
            catch (Exception e)
            {
                lock (MainBinding._locker)
                {
                    file.Error = e.Message;

                    if (e.Message.Contains("The request was aborted"))
                    {
                        file.Error = null;
                    }
                }
                    
            }
        }



        public async Task IndexCssContent(string cssContent, string contextUrl)
        {
            List<string> resourceUrls = CssParser.getCssUrls(cssContent, contextUrl);

            foreach (string localPath in resourceUrls)
            {
                if (!string.IsNullOrEmpty(localPath))
                {
                    FileBinding fileBinding = new FileBinding();

                    string url = PathOperation.MakeUrl(contextUrl, localPath);

                    fileBinding.Url = url;



                    fileBinding.Domain = Domain;
                    fileBinding.FileLocation = PathOperation.ChangeUrlToWindowsPath(url, PathOperation.GetFolderFromDomain(fileBinding.Domain)); ;


                    fileBinding.Downloading = 0;

                    lock (MainBinding._locker) {
                        if (!MainBinding.FileBindings.Contains(fileBinding))
                        {
                            MainBinding.FileBindings.Add(fileBinding);
                        }
                    }
                    
                    
                }
            }

            lock (MainBinding._locker)
            {
                MainBinding.FilesCount = MainBinding.FileBindings.Count;

            }

        }





        public void CalculateTotalProgress()
        {
            lock (MainBinding._locker) {
                int count = MainBinding.FileBindings.Count;
                int sum = 0;

                MainBinding.DownloadFailure = 0;
                MainBinding.DownloadSuccess = 0;
                foreach (var file in MainBinding.FileBindings)
                {
                    if (file.Error != null || file.Size == -1)
                        MainBinding.DownloadFailure += 1;
                    if (file.Downloading == 100)
                        MainBinding.DownloadSuccess += 1;
                    if (file.Error != null || file.Downloading == 100)
                        sum += 1;
                }
                double progress = ((double)sum / (double)count) * 100;
                if (progress == 100)
                    MainBinding.Running = false;
                MainBinding.TotalProgressBar = (int)progress;
            }
            
        }









    }
}
