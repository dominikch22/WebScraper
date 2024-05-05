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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainBinding
            {
                Urls = "Initial URL",
                FileBindings = new ObservableCollection<FileBinding>
                {
                    new FileBinding { FileName = "File1.txt", Size = 1024, Downloading = 10 },
                    new FileBinding { FileName = "File2.txt", Size = 2048, Downloading = 10 }
                }
            };
          
        }


    }
}
