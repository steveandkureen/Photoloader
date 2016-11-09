using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Linq;
using PhotoServerTools;

namespace PhotoLoader4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        PhotoLoaderSettings Settings { get; set; }
        private ImageLoader imageLoader;
        Task mainTask;
        CancellationTokenSource cancellationTokenSource;

        bool errorOccured;

        string imageListFilePath;
        string errorFilePath;
        string settingsFile;
        private string sourceFolder;

        public MainWindow()
        {
            InitializeComponent();
            sourceFolder = "";
        }

        void LoadSettings()
        {
            imageListFilePath = Path.Combine(App.Current.ApplicationDataFolder, "ImageList.txt");
            errorFilePath = Path.Combine(App.Current.ApplicationDataFolder, "Error.txt");
            settingsFile = Path.Combine(App.Current.ApplicationDataFolder, "PhotoLoaderSettings.xml");

            Settings = SettingsWindow.LoadSettings(settingsFile);

            deleteOriginalCheckBox.IsChecked = true;

        }

        private DialogResult OpenFolderDialog(ref string startFolder)
        {
            if (string.IsNullOrEmpty(startFolder))
                startFolder = "";

            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                SelectedPath = startFolder
            };

            var res = dialog.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                startFolder = dialog.SelectedPath;
            }

            return res;
        }

        private void SourceButtonClick(object sender, RoutedEventArgs e)
        {
            var sourceFolder = this.sourceFolder;
            if (OpenFolderDialog(ref sourceFolder) == System.Windows.Forms.DialogResult.OK)
            {
                this.sourceFolder = sourceFolder;
            }
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (var folder in Settings.SourcePaths.Split(';'))
            {
                if (Directory.Exists(folder))
                {
                    sourceFolder = folder;
                    break;
                }
            }

            // Verify Directories exist
            if (string.IsNullOrEmpty(sourceFolder))
            {
                SetStatusText("Source folder not found.");
                return;
            }

            if (!Directory.Exists(Settings.DestPath))
            {
                SetStatusText("Destination folder not found.");
                return;
            }

            errorOccured = false;
            StartButton.IsEnabled = false;

            SetStatusText("Getting image list.");

            var fileExentions = new List<string> { "*.JPG", "*.PNG", "*.BMP", "*.MOV", "*.MP4" };
            var imagePaths = new List<PhotoInfo>();
            foreach (var fileExention in fileExentions)
            {
                var files = Directory.EnumerateFiles(sourceFolder, fileExention, SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    imagePaths.Add(new PhotoInfo { PhotoId = Guid.NewGuid(), PhotoPath = file });
                }
            }


            if (imagePaths.Count == 0)
            {
                SetStatusText("No images found");
                StartButton.IsEnabled = true;
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();

            imageLoader = new ImageLoader { Settings = Settings, ImagePaths = imagePaths, CancellationToken = cancellationTokenSource.Token };

            FileList.ItemsSource = imageLoader.ImagePaths;
            FileList.ScrollIntoView(imageLoader.ImagePaths[0]);
            var deleteOriginal = deleteOriginalCheckBox.IsChecked != null && deleteOriginalCheckBox.IsChecked.Value;

            var photoLoaderAction = new Action(() =>
                                                  {
                                                      List<PhotoInfo> destPaths;
                                                   // Get Images from Camera

                                                   destPaths = GetImagesFromCamera(deleteOriginal);

                                                      if (imageLoader.CancellationToken.IsCancellationRequested)
                                                      {
                                                          SetStatusText("Cancelling");
                                                      }

                                                      imageLoader.ImagePaths = destPaths;

                                                   // Start Ftp to Site
                                                   FtpImagesToSite();
                                                  });


            mainTask = new Task(photoLoaderAction, cancellationTokenSource.Token);
            mainTask.Start();
        }

        void ImageLoaderDownloadImageFromSourceComplete(object sender, ImageUpdateEvent evt)
        {
            var image = imageLoader.ImagePaths.Single(i => i.PhotoId == evt.PhotoInfo.PhotoId);
            image.IsComplete = evt.PhotoInfo.IsComplete;
            image.InProgress = evt.PhotoInfo.InProgress;
            image.IsError = evt.PhotoInfo.IsError;

            if (!imageLoader.CancellationToken.IsCancellationRequested)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                                                {
                                                    FileList.SelectedItem = image;
                                                    FileList.ScrollIntoView(image);
                                                }));
            }
        }


        void FtpImagesToSite()
        {
            SetStatusText("Uploading Images to SmugMug.com");

            // Write initial image list file to recover from errors.
            WriteRestoreFile();

            if (cancellationTokenSource.Token.IsCancellationRequested)
                return;

            // Reset Image list
            Dispatcher.BeginInvoke(new Action(() =>
            {
                FileList.ItemsSource = imageLoader.ImagePaths;
                FileList.ScrollIntoView(imageLoader.ImagePaths[0]);
            }));


            imageLoader.UploadImageFromSourceComplete += ImageLoaderUploadImageFromSourceComplete;

            imageLoader.UploadToSite();
            UploadToSiteComplete();
        }

        List<PhotoInfo> GetImagesFromCamera(bool deleteOriginal)
        {

            imageLoader.DownloadImageFromSourceComplete += ImageLoaderDownloadImageFromSourceComplete;


            SetStatusText("Downloading From Camera.");
            var destPaths = new List<PhotoInfo>();
            try
            {
                destPaths = imageLoader.DownLoadFromSource(deleteOriginal);
            }
            catch (CancelationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                SetStatusText("Downloading from camera failed.");
                using (var writer = File.CreateText(errorFilePath))
                {
                    writer.Write($"{ex} \n\n Stack:\n {ex.StackTrace}");
                }
            }
            return destPaths;
        }

        void ImageLoaderUploadImageFromSourceComplete(object sender, ImageUpdateEvent evt)
        {
            SetStatusText("Uploading to SmugMug.com");

            var image = imageLoader.ImagePaths.Single(i => i.PhotoId == evt.PhotoInfo.PhotoId);
            if (evt.PhotoInfo.Error == null)
            {
                image.IsComplete = evt.PhotoInfo.IsComplete;
                image.InProgress = evt.PhotoInfo.InProgress;
            }
            else
            {
                image.IsError = true;
                image.Error = evt.PhotoInfo.Error;
                errorOccured = true;
            }

            if (!imageLoader.CancellationToken.IsCancellationRequested)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                                                {
                                                    FileList.SelectedItem = image;
                                                    FileList.ScrollIntoView(image);
                                                }));
            }

            if (image.IsComplete)
            {
                WriteRestoreFile();
            }
        }


        void WriteRestoreFile()
        {
            try
            {
                if (IsWriting)
                    return;

                IsWriting = true;
                using (var writer = File.CreateText(imageListFilePath))
                {
                    foreach (var file in imageLoader.ImagePaths)
                    {
                        if ((!file.IsComplete) || (file.IsError))
                        {
                            writer.WriteLine($"{file.PhotoId},{file.PhotoPath},{file.PhotoDate}");
                        }
                    }
                }
                IsWriting = false;
            }
            catch (Exception)
            {

            }
        }


        private void UploadToSiteComplete()
        {
            if (!errorOccured)
            {
                File.Delete(imageListFilePath);
                SetStatusText("Upload Complete");
            }
            else
            {
                SetStatusText("Upload Completed with errors");
            }
        }

        private void SetStatusText(string text)
        {
            if ((cancellationTokenSource != null) && (cancellationTokenSource.Token.IsCancellationRequested))
                text = "Cancelling";

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.BeginInvoke(new Action(() =>
                                                 {
                                                     StatusLine.Text = text;
                                                 }));
            }
            else
            {
                StatusLine.Text = text;
            }
        }

        void RestartUpload()
        {
            StartButton.IsEnabled = false;
            var destPaths = new List<PhotoInfo>();
            using (var reader = File.OpenText(imageListFilePath))
            {
                var fileText = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(fileText))
                {
                    foreach (var image in fileText.Split('\n'))
                    {
                        if (!string.IsNullOrEmpty(image))
                        {
                            var temp = image.Trim().Split(',');
                            var info = new PhotoInfo { PhotoId = Guid.Parse(temp[0]), PhotoPath = temp[1], PhotoDate = DateTime.Parse(temp[2]) };
                            destPaths.Add(info);
                        }
                    }
                }
            }

            if (destPaths.Count > 0)
            {
                SetStatusText("Resuming upload...");
                Action action = FtpImagesToSite;
                cancellationTokenSource = new CancellationTokenSource();
                mainTask = new Task(action, cancellationTokenSource.Token);

                imageLoader = new ImageLoader { Settings = Settings, ImagePaths = destPaths, CancellationToken = cancellationTokenSource.Token };
                mainTask.Start();
            }
            else
            {
                File.Delete(imageListFilePath);
            }
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void SettingsButtonClick(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(settingsFile);
            settingsWindow.ShowDialog();
            Settings = settingsWindow.Settings;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(imageListFilePath))
            {
                RestartUpload();
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (cancellationTokenSource != null)
                cancellationTokenSource.Cancel();

            if (mainTask != null)
            {
                e.Cancel = true;
                var endTask = new Task(() =>
                                          {
                                              mainTask.Wait();
                                              mainTask = null;
                                              Dispatcher.BeginInvoke(new Action(Close));
                                          });
                endTask.Start();
            }
            else
            {
                if (imageLoader != null)
                {
                    imageLoader.UploadImageFromSourceComplete -= ImageLoaderUploadImageFromSourceComplete;
                    imageLoader.DownloadImageFromSourceComplete -= ImageLoaderDownloadImageFromSourceComplete;
                    imageLoader = null;
                }
            }

        }

        public bool IsWriting { get; set; }

        private void TestButton_OnClick(object sender, RoutedEventArgs e)
        {
            var itemsSource = new List<PhotoInfo>();
            itemsSource.Add(new PhotoInfo {PhotoPath = @"C:\myImage.jpg", IsComplete = true});
            itemsSource.Add(new PhotoInfo { PhotoPath = @"C:\myImage2.jpg", InProgress = true});
            itemsSource.Add(new PhotoInfo { PhotoPath = @"C:\myImage3.jpg", IsError = true});
            itemsSource.Add(new PhotoInfo { PhotoPath = @"C:\myImage4.jpg" });
            FileList.ItemsSource = itemsSource;
        }
    }
}

