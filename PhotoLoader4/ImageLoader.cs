using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PhotoServerTools;
using SmugMugWrapper;
using System.Linq;
using System.Xml.Serialization;
using SmugMugWrapper.V2;

namespace PhotoLoader4
{
    public class ImageLoader
    {
        public List<PhotoInfo> ImagePaths { get; set; }
        public CancellationToken CancellationToken { get; set; }

        public PhotoLoaderSettings Settings { get; set; }

        protected PhotoInfo CurrentUpload { get; set; }
        public event ImageLoaderEvent DownloadImageFromSourceComplete;
        public event ImageLoaderEvent UploadImageFromSourceComplete;


        public List<PhotoInfo> DownLoadFromSource(bool deleteOriginals)
        {
            var destPath = new List<PhotoInfo>();

            foreach (var imagePath in ImagePaths)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    return destPath;
                }

                var imageDateFound = false;
                try
                {
                    imagePath.PhotoDate = ImageHandler.GetImageDate(imagePath.PhotoPath);
                    imageDateFound = true;
                }
                catch (Exception)
                {
                    imagePath.PhotoDate = File.GetLastWriteTime(imagePath.PhotoPath);
                }

                string newPath = Path.Combine(Settings.DestPath, "_0_" + imagePath.PhotoDate.ToString("yyyy"));
                newPath = Path.Combine(newPath, imagePath.PhotoDate.ToString("_MM_MMMM"));
                newPath = Path.Combine(newPath, "date-" + imagePath.PhotoDate.ToString("yyyy-MM-dd"));

                if (imageDateFound)
                    newPath = Path.Combine(newPath, imagePath.PhotoDate.ToString("MM-dd-yyyy hh-mm-ss tt") + Path.GetExtension(imagePath.PhotoPath));
                else
                    // ReSharper disable AssignNullToNotNullAttribute
                    newPath = Path.Combine(newPath, Path.GetFileName(imagePath.PhotoPath));
                // ReSharper restore AssignNullToNotNullAttribute

                int count = 0;
                var tempPath = newPath;
                while (File.Exists(tempPath))
                {
                    count++;
                    // ReSharper disable AssignNullToNotNullAttribute
                    tempPath = Path.Combine(Path.GetDirectoryName(newPath),
                                           // ReSharper restore AssignNullToNotNullAttribute
                                           Path.GetFileNameWithoutExtension(newPath) + "_" + count +
                                           Path.GetExtension(newPath));
                }

                newPath = tempPath;

                ImageHandler.TransferLocal(imagePath.PhotoPath, newPath, !deleteOriginals);

                destPath.Add(new PhotoInfo { PhotoId = imagePath.PhotoId, PhotoPath = newPath, PhotoDate = imagePath.PhotoDate });

                imagePath.IsComplete = true;

                DownloadImageFromSourceComplete?.Invoke(this, new ImageUpdateEvent { PhotoInfo = imagePath });
            }

            return destPath;
        }

        private SmugMugApiV2 CreateSmugMugApi()
        {
            var settingsPath = Path.Combine(App.Current.ApplicationDataFolder, "SmugMug.dat");
            var token = new OAuthToken();
            if (File.Exists(settingsPath))
            {
                using (var writer = File.Open(settingsPath, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(OAuthToken));
                    token = (OAuthToken)serializer.Deserialize(writer);
                }
            }

            return new SmugMugApiV2(token);
        }


        public void UploadToSite()
        {
            var smugApi = CreateSmugMugApi();

            foreach (var imageInfo in ImagePaths)
            {
                if (CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    CurrentUpload = imageInfo;
                    CurrentUpload.InProgress = true;
                    UploadImageFromSourceComplete(this, new ImageUpdateEvent { PhotoInfo = CurrentUpload });

                    var imageAlbumName = imageInfo.PhotoDate.ToString("MMMM dd, yyyy");
                    var imageYear = imageInfo.PhotoDate.ToString("yyyy");
                    var imageMonth = imageInfo.PhotoDate.ToString("MMMM");

                    var rootNode = smugApi.FindRootNode();
                    var year = smugApi.FindYear(imageYear);
                    if (year == null)
                    {
                        year = smugApi.AddNode(rootNode, new NewNode {Name = imageYear, Type = "Folder"});
                    }

                    var month = smugApi.FindMonth(year, imageMonth);
                    if (month == null)
                    {
                        month = smugApi.AddNode(year, new NewNode {Name = imageMonth, Type = "Folder"});
                    }

                    var day = smugApi.FindDay(month, imageAlbumName);
                    if (day == null)
                    {
                        day = smugApi.AddNode(month, new NewNode {Name = imageAlbumName, Type = "Album"});
                    }

                    UploadComplete = false;
                    smugApi.UploadImage(imageInfo.PhotoPath, day);
                    CurrentUpload.IsComplete = true;
                    UploadImageFromSourceComplete(this, new ImageUpdateEvent { PhotoInfo = CurrentUpload });
                }
                catch (CancelationException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (UploadImageFromSourceComplete != null)
                    {
                        CurrentUpload.Error = ex;
                        CurrentUpload.IsError = true;
                        UploadImageFromSourceComplete(this, new ImageUpdateEvent { PhotoInfo = CurrentUpload });
                    }
                }
            }
        }

        protected bool UploadComplete { get; set; }
       
    }

    public delegate void ImageLoaderEvent(object sender, ImageUpdateEvent evt);

    public class ImageUpdateEvent : EventArgs
    {
        public PhotoInfo PhotoInfo { get; set; }
    }



    public class CancelationException : Exception
    {

    }
}
