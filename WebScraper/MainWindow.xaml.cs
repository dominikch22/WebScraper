using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();

            MainBinding = new MainBinding();

            DataContext = MainBinding;

            List<string> urls = new List<string> { "https://akademiabialska.pl", "https://akademiabialska.pl/aktualnosci/x-ogolnopolska-konferencja-studenckich-kol-naukowych-151.html", "https://rekrutacja.akademiabialska.pl/aktualnosci/fotorelacja-z-dnia-otwartego-2024-12.html" };

            MainBinding.Urls = string.Join("\r\n\r\n", urls);
            MainBinding.Domain = "alamakota.pl";

            DownloadService downloadService = new DownloadService(MainBinding);
            downloadService.Start();
            /*downloadService.IndexAndDownloadAllUrls();
            downloadService.DownloadAllResources();*/
            /* WebScrapper web = new WebScrapper(mainBinding, "https://akademiabialska.pl", "alamakota.pl");
             web.DownloadAndIndexReosurces();

             WebScrapper web1 = new WebScrapper(mainBinding, "https://akademiabialska.pl/aktualnosci/x-ogolnopolska-konferencja-studenckich-kol-naukowych-151.html", "alamakota.pl");
             web1.DownloadAndIndexReosurces();

             WebScrapper web2 = new WebScrapper(mainBinding, "https://rekrutacja.akademiabialska.pl/aktualnosci/fotorelacja-z-dnia-otwartego-2024-12.html", "alamakota.pl");
             web2.DownloadAndIndexReosurces();*/

            //web2.DownloadAllResources();
        }




    }
}
