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
            //LocalHttpServer.Start();
            MainBinding = new MainBinding();

            DataContext = MainBinding;

            List<string> urls = new List<string> { "https://akademiabialska.pl"};
            //https://rekrutacja.akademiabialska.pl/oferta/informatyka-3.html
            //, "https://akademiabialska.pl/aktualnosci/x-ogolnopolska-konferencja-studenckich-kol-naukowych-151.html", "https://rekrutacja.akademiabialska.pl/aktualnosci/fotorelacja-z-dnia-otwartego-2024-12.html" 
            MainBinding.Urls = string.Join("\r\n\r\n", urls);
            MainBinding.Domain = "kott";

          
        }

        public void StartClicked(object sender, RoutedEventArgs e) {
            //DownloadService = new DownloadService(MainBinding);
            /*  WebScrapper webScrapper = new WebScrapper(MainBinding, "https://akademiabialska.pl", "kott");
              webScrapper.IndexAndDownload();*/
            //DownloadService.Start();
            Thread newThread = new Thread(new ParameterizedThreadStart(LocalHttpServer.Start));

            // Przekazanie argumentu do nowego wątku
            newThread.Start(MainBinding.Domain);
            // Uruchomienie nowego wątku


            DownloadService = new DownloadService(MainBinding);
            DownloadService.Start();
        }

        public void StopClicked(object sender, RoutedEventArgs e)
        {
            DownloadService.StopDownloading();
        }

        public void ContinueClicked(object sender, RoutedEventArgs e)
        {
            DownloadService.Start();
        }


    }
}
