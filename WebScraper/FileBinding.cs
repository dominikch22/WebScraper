﻿using System.ComponentModel;

namespace WebScraper
{
    public class FileBinding : INotifyPropertyChanged
    {
        private string _url;

        public string Url
        {
            get { return _url; }
            set
            {

                _url = value;
                OnPropertyChanged(nameof(Url));

            }
        }

        private int _downloading;
        public int Downloading
        {
            get { return _downloading; }
            set
            {
                _downloading = value;
                OnPropertyChanged(nameof(Downloading));
            }
        }

        private string _fileName;
        public string FileLocation
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileLocation));
            }
        }

        private long _size;
        public long Size
        {
            get { return _size; }
            set
            {
                _size = value;
                OnPropertyChanged(nameof(Size));
            }
        }

        private long _downloadedBytes;
        public long DownloadedBytes
        {
            get { return _downloadedBytes; }
            set
            {
                _downloadedBytes = value;
                OnPropertyChanged(nameof(DownloadedBytes));
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

        public string Domain;
        public string Path;

        public FileBinding(string url, long size, int downloading, string domain) { 
            Url = url;
            Size = size;
            Downloading = downloading;
            Domain = domain;
        }



        public FileBinding() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            return obj is FileBinding binding &&
                   Url.Equals(binding.Url);
        }
    }
}
