using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PhotoServerTools
{
    public class PhotoInfo : INotifyPropertyChanged
    {
        private bool isComplete;
        private bool inProgress;
        private bool isError;
        public Guid PhotoId { get; set; }
        public string PhotoPath { get; set; }
        public string PhotoFileName { get { return Path.GetFileName(PhotoPath); } }
        public Exception Error { get; set; }
        public DateTime PhotoDate { get; set; }

        public bool InProgress
        {
            get { return inProgress; }
            set { inProgress = value; OnPropertyChanged("StatusImage"); OnPropertyChanged("NotInProgress"); OnPropertyChanged("ShowProgress"); OnPropertyChanged("InProgress"); }
        }

        public bool ShowProgress => InProgress && !IsComplete;

        public bool NotShowProgress => !ShowProgress;

        public bool IsComplete
        {
            get { return isComplete; }
            set { isComplete = value; OnPropertyChanged("StatusImage"); OnPropertyChanged("NotInProgress"); OnPropertyChanged("ShowProgress"); OnPropertyChanged("InProgress"); }
        }

        public bool IsError
        {
            get { return isError; }
            set { isError = value; OnPropertyChanged("StatusImage"); OnPropertyChanged("NotInProgress"); }
        }

        public string StatusImage
        {
            get
            {
                if (IsError)
                {
                    return "pack://application:,,,/Resources/error.png";
                }
                if (IsComplete)
                {
                    return "pack://application:,,,/Resources/done.png";
                }
                //if (InProgress)
                //{
                //    return "pack://application:,,,/Resources/spinner.gif";
                //}

                return "pack://application:,,,/Resources/clock.jpg";
            }
        }

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public delegate void ImageLoaderEvent(object sender, ImageUpdateEvent evt);

    public class ImageUpdateEvent : EventArgs
    {
        public PhotoInfo PhotoInfo { get; set; }
    }
}
