using System.Collections.ObjectModel;
using System.ComponentModel;

namespace WebScraper
{
    public class MainBinding : INotifyPropertyChanged
    {
        private readonly object _locker = new object();

        private string _urls;
        public string Urls
        {
            get
            {
                lock (_locker)
                {
                    return _urls;
                }
            }
            set
            {
                lock (_locker)
                {
                    _urls = value;
                    OnPropertyChanged(nameof(Urls));
                }
            }
        }


        private ObservableCollection<FileBinding> _fileBindings;
        public ObservableCollection<FileBinding> FileBindings
        {
            get
            {
                lock (_locker)
                {
                    return _fileBindings;
                }
            }
            set
            {
                lock (_locker)
                {
                    _fileBindings = value;
                    OnPropertyChanged(nameof(FileBindings));

                }
            }
        }

        private int _totalProgressBar;
        public int TotalProgressBar
        {
            get
            {
                lock (_locker)
                {
                    return _totalProgressBar;
                }
            }
            set
            {
                lock (_locker)
                {
                    _totalProgressBar = value;
                    OnPropertyChanged(nameof(TotalProgressBar));

                }
            }
        }

        /*private string _downloadedFile;
        public string DownloadedFile
        {
            get { return _downloadedFile; }
            set
            {
                _downloadedFile = value;
                OnPropertyChanged(nameof(DownloadedFile));
            }
        }*/

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

        private bool _serverPaths;
        public bool ServerPaths
        {
            get { return _serverPaths; }
            set
            {

                _serverPaths = value;
                OnPropertyChanged(nameof(ServerPaths));

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
            get
            {
                lock (_locker)
                {
                    return _downloadSuccess;
                }
            }
            set
            {
                lock (_locker)
                {
                    _downloadSuccess = value;
                    OnPropertyChanged(nameof(DownloadSuccess));

                }
            }
        }

        public int DownloadFailure
        {
            get
            {
                lock (_locker)
                {
                    return _downloadFailure;
                }
            }
            set
            {
                lock (_locker)
                {
                    _downloadFailure = value;
                    OnPropertyChanged(nameof(DownloadFailure));

                }
            }
        }


        private int _filesCount;

        public int FilesCount
        {
            get
            {
                lock (_locker)
                {
                    return _filesCount;
                }
            }
            set
            {
                lock (_locker)
                {
                    _filesCount = value;
                    OnPropertyChanged(nameof(FilesCount));

                }
            }
        }

        private string _error;

        public string Error
        {
            get { return _error; }
            set
            {

                _error = value;
                OnPropertyChanged(nameof(Error));

            }
        }

        public bool Running;
        public MainBinding()
        {
            Urls = "";
            FileBindings = new ObservableCollection<FileBinding>();
            //FileProgressBar = 0;
            ServerPaths = true;
            TotalProgressBar = 0;
            Running = false;
            ShorterDirectories = true;
            //DownloadedFile = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
