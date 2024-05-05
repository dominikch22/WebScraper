using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraper
{
    public class FileBinding : INotifyPropertyChanged
    {
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
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged(nameof(FileName));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
