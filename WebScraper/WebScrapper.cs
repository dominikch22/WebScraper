using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;
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
            if (!MainBinding.FileBindings.Contains(file))
                MainBinding.FileBindings.Add(file);
            else
                Console.WriteLine("");

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

        public void IndexResources(string content)
        {

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);

            HtmlNodeCollection nodes = htmlDocument.DocumentNode.SelectNodes("//img|//link|//script");
            //HtmlNodeCollection htmlNodes = GetImgLinkScriptNodesFromHtml(content);


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
                    string url = CssParser.MakeUrl(Url, localPath);

                    string shorterLocalPath = localPath.MakeShorterLocalPath();
                    node.SetAttributeValue(atrributeName, shorterLocalPath);
                    //node.SetAttributeValue(atrributeName, "chuj");



                    string windowsPath = CssParser.ChangeUrlToWindowsPath(url, Domain);
                    string shorterWindowsPath = windowsPath.MakeShorterWindowsPath();


                    fileBinding.Url = url;

                    fileBinding.FileLocation = shorterWindowsPath;
                    fileBinding.Domain = Domain;

                    fileBinding.Downloading = 0;

                    if (!MainBinding.FileBindings.Contains(fileBinding)) {
                        MainBinding.FileBindings.Add(fileBinding);

                        //DownloadResource(fileBinding);
                    }
                }
            }

            string windowspHtlmPath = CssParser.ChangeUrlToWindowsPath(Url, Domain);
            Directory.CreateDirectory(Path.GetDirectoryName(windowspHtlmPath));
            htmlDocument.Save(windowspHtlmPath);
        }

       /* public HtmlNodeCollection GetImgLinkScriptNodesFromHtml(string htmlContent)
        {

            return nodes;
        }*/

        public async Task DownloadResource(FileBinding file)
        {
            if (file.Downloading == 100 || file.Error != null)
                return;
            try
            {
                using (Client = new WebClient())
                {
                    Client.DownloadProgressChanged += (sender, e) =>
                    {
                        file.Downloading = e.ProgressPercentage;
                        file.Size = e.TotalBytesToReceive;
                    };

                    if (file.Url == null)
                        return;


                    string path = CssParser.ChangeUrlToWindowsPath(file.Url, file.Domain);

                    Directory.CreateDirectory(Path.GetDirectoryName(path));

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

                    /*string content = await client.DownloadStringTaskAsync(file.Url);
                    File.WriteAllText(combinedPath, content);*/


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

            foreach (string localPath in resourceUrls)
            {
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

                    if (!MainBinding.FileBindings.Contains(fileBinding))
                    {
                        MainBinding.FileBindings.Add(fileBinding);
                        //await DownloadResource(fileBinding);
                    }
                    else
                        Console.WriteLine("");
                }
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
