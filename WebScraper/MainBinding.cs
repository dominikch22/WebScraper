using System.Collections.ObjectModel;
using System.ComponentModel;

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

        private string _domain;
        public string Domain
        {
            get { return _domain; }
            set
            {
                _domain = value;
                OnPropertyChanged(nameof(Domain));
            }
        }

        private bool _shorterDirectories;
        public bool ShorterDirectories
        {
            get { return _shorterDirectories; }
            set
            {

                _shorterDirectories = value;
                OnPropertyChanged(nameof(ShorterDirectories));

            }
        }

        private int _downloadSuccess;
        private int _downloadFailure;

        public int DownloadSuccess
        {
            get { return _downloadSuccess; }
            set
            {
                
                    _downloadSuccess = value;
                    OnPropertyChanged(nameof(DownloadSuccess));
                
            }
        }

        public int DownloadFailure
        {
            get { return _downloadFailure; }
            set
            {
               
                    _downloadFailure = value;
                    OnPropertyChanged(nameof(DownloadFailure));
                
            }
        }
        public MainBinding()
        {
            Urls = "";
            FileBindings = new ObservableCollection<FileBinding>();
            //FileProgressBar = 0;
            ShorterDirectories = true;
            TotalProgressBar = 0;
            //DownloadedFile = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
