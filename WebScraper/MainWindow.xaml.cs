using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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


            List<string> urls = new List<string> { "https://akademiabialska.pl"};
            
            MainBinding.Urls = string.Join("\r\n\r\n", urls);
            MainBinding.Domain = "http://kott.pl";

            DownloadService = new DownloadService(MainBinding);



        }

        public void StartClicked(object sender, RoutedEventArgs e) {
            try {
                MainBinding.Running = false;
                DownloadService.StopDownloading();

                
                    DownloadService = new DownloadService(MainBinding);

                    MainBinding.FileBindings.Clear(); 
                    DownloadService.Start();
                    MainBinding.Running = true;

                    LocalHttpServer.Start(PathOperation.GetFolderFromDomain(MainBinding.Domain));

                    MainBinding.Error = "";
                

            }catch (Exception ex)
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

            MainBinding.Running = false;
            DownloadService.StopDownloading();
           
                DownloadService = new DownloadService(MainBinding);
                DownloadService.Start();
                MainBinding.Running = true;
            
        }


    }
}
