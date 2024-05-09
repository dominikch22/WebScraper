using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace WebScraper
{
    public partial class MainWindow : Window
    {
        public MainBinding MainBinding;
        public DownloadService DownloadService;

        public MainWindow()
        {
            InitializeComponent();
            MainBinding = new MainBinding();

            DataContext = MainBinding;

            BindingOperations.EnableCollectionSynchronization(MainBinding.FileBindings, MainBinding._locker);


            List<string> urls = new List<string> { "https://akademiabialska.pl" };

            MainBinding.Urls = string.Join("\r\n\r\n", urls);
            MainBinding.Domain = "http://localhost:8080";

            DownloadService = new DownloadService(MainBinding);



        }

        public void StartClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                MainBinding.Running = false;
                DownloadService.StopDownloading();


                DownloadService = new DownloadService(MainBinding);

                MainBinding.FileBindings.Clear();
                DownloadService.Start();
                MainBinding.Running = true;

                LocalHttpServer.Start(PathOperation.GetFolderFromDomain(MainBinding.Domain));

                MainBinding.Error = "";


            }
            catch (Exception ex)
            {
                MainBinding.Error = ex.Message;
                MainBinding.Running = false;
            }




        }

        public void StopClicked(object sender, RoutedEventArgs e)
        {
            MainBinding.Running = false;
            DownloadService.StopDownloading();
        }

        public void ContinueClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                MainBinding.Running = false;
                DownloadService.StopDownloading();

                DownloadService = new DownloadService(MainBinding);
                DownloadService.Start();
                MainBinding.Running = true;
            }
            catch (Exception ex)
            {
                MainBinding.Error = ex.Message;
                MainBinding.Running = false;
            }


        }


    }
}
