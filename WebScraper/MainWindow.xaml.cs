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

            List<string> urls = new List<string> { "https://akademiabialska.pl"};
            //https://rekrutacja.akademiabialska.pl/oferta/informatyka-3.html
            //, "https://akademiabialska.pl/aktualnosci/x-ogolnopolska-konferencja-studenckich-kol-naukowych-151.html", "https://rekrutacja.akademiabialska.pl/aktualnosci/fotorelacja-z-dnia-otwartego-2024-12.html" 
            MainBinding.Urls = string.Join("\r\n\r\n", urls);
            MainBinding.Domain = "kott";

          
        }

        public void StartClicked(object sender, RoutedEventArgs e) {
            if (!MainBinding.Running) {
                MainBinding.FileBindings = new ObservableCollection<FileBinding>();
                DownloadService = new DownloadService(MainBinding);
                DownloadService.Start();
                MainBinding.Running = true;

                LocalHttpServer.Start(PathOperation.GetFolderFromDomain(MainBinding.Domain));
            }
            

           
        }

        public void StopClicked(object sender, RoutedEventArgs e)
        {
            MainBinding.Running = false;
            DownloadService.StopDownloading();
        }

        public void ContinueClicked(object sender, RoutedEventArgs e)
        {
            if (!MainBinding.Running)
            {
                DownloadService = new DownloadService(MainBinding);
                DownloadService.Start();
                MainBinding.Running = true;
            }
        }


    }
}
