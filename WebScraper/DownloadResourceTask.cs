using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class DownloadResourceTask
    {
        public WebClient Client { get; set; }
        public FileBinding File;
        public MainBinding MainBinding { get; set; }


        public async Task StartDownloading() {
            if (File.Downloading == 100 || File.Error != null)
                return;
            try
            {
                using (WebClient client = new WebClient())
                {

                    client.DownloadProgressChanged += (sender, e) =>
                    {
                        File.Downloading = e.ProgressPercentage;
                        File.Size = e.TotalBytesToReceive;
                    };

                    if (File.Url == null)
                        return;


                    Uri uri = new Uri(File.Url);

                    string localPath = uri.LocalPath.Replace("/", "\\");
                    string currentDirectory = @"C:\webscraper\";


                    if (MainBinding.ServerPaths && localPath.Length > 100)
                        localPath = localPath.MakeShorterPath();
                    string combinedPath = currentDirectory + localPath;

                    combinedPath = combinedPath.Replace("\\\\", "\\");



                    Directory.CreateDirectory(Path.GetDirectoryName(combinedPath));


                    await Client.DownloadFileTaskAsync(File.Url, combinedPath);

                    MainBinding.DownloadSuccess += 1;
                    CalculateTotalProgress();
                }
            }
            catch (Exception e)
            {
                MainBinding.DownloadFailure += 1;

                File.Error = e.Message;
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
        public void StopDownloading() {
            Client.CancelAsync();
        }
    }
}
