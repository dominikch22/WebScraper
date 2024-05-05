using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class MainBinding : INotifyPropertyChanged
    {
        private string _urls;
        public string Urls
        {
            get { return _urls; }
            set
            {
                _urls = value;
                OnPropertyChanged(nameof(Urls));
            }
        }

        private ObservableCollection<FileBinding> _fileBindings;
        public ObservableCollection<FileBinding> FileBindings
        {
            get { return _fileBindings; }
            set
            {
                _fileBindings = value;
                OnPropertyChanged(nameof(FileBindings));
            }
        }

        private int _fileProgressBar;
        public int FileProgressBar
        {
            get { return _fileProgressBar; }
            set
            {
                _fileProgressBar = value;
                OnPropertyChanged(nameof(FileProgressBar));
            }
        }

        private int _totalProgressBar;
        public int TotalProgressBar
        {
            get { return _totalProgressBar; }
            set
            {
                _totalProgressBar = value;
                OnPropertyChanged(nameof(TotalProgressBar));
            }
        }

        private string _downloadedFile;
        public string DownloadedFile
        {
            get { return _downloadedFile; }
            set
            {
                _downloadedFile = value;
                OnPropertyChanged(nameof(DownloadedFile));
            }
        }
        public MainBinding()
        {
            Urls = string.Empty;
            FileBindings = new ObservableCollection<FileBinding>();
            FileProgressBar = 0;
            TotalProgressBar = 0;
            DownloadedFile = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
