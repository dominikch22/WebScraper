﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace WebScraper
{
    public class MainBinding : INotifyPropertyChanged
    {

        public static object _locker = new object();

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


        private ThreadSafeObservableCollection<FileBinding> _fileBindings;
        public ThreadSafeObservableCollection<FileBinding> FileBindings
        {
            get { return _fileBindings; }
            set
            {
                _fileBindings = value;
                OnPropertyChanged(nameof(FileBindings));
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

        /*private bool _serverPaths;
        public bool ServerPaths
        {
            get { return _serverPaths; }
            set
            {

                _serverPaths = value;
                OnPropertyChanged(nameof(ServerPaths));

            }
        }*/

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


        private int _filesCount;

        public int FilesCount
        {
            get { return _filesCount; }
            set
            {

                _filesCount = value;
                OnPropertyChanged(nameof(FilesCount));

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
            FileBindings = new ThreadSafeObservableCollection<FileBinding>();
            //FileProgressBar = 0;
            //ServerPaths = true;
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
