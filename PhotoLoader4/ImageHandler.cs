using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Encoder = System.Drawing.Imaging.Encoder;

namespace PhotoLoader4
{
   

   public static class ImageHandler
   {
      public static DateTime GetImageDate(string imagePath)
      {
         using (var image = new Bitmap(imagePath))
         {
            foreach (PropertyItem item in image.PropertyItems)
            {
               if (item.Id == 36867)
               {
                  var enc = new ASCIIEncoding();
                  string tempTime = enc.GetString(item.Value).Replace("\\0", "");
                  string[] dateData = tempTime.Split(' ');
                  string[] date = dateData[0].Split(':');
                  string[] hours = dateData[1].Split(':');
                  return new DateTime(Convert.ToInt32(date[0]), Convert.ToInt32(date[1]), Convert.ToInt32(date[2]),
                                       Convert.ToInt32(hours[0]), Convert.ToInt32(hours[1]), Convert.ToInt32(hours[2]));
               }
            }
         }

         throw new Exception("Image date not found.");
      }

      public static void TransferLocal(string oldPath, string newPath, bool copyOnly)
      {
// ReSharper disable AssignNullToNotNullAttribute
         Directory.CreateDirectory(Path.GetDirectoryName(newPath));
// ReSharper restore AssignNullToNotNullAttribute

         if (!copyOnly)
         {
            try
            {
               File.Move(oldPath, newPath);
            }
            catch (Exception)
            {
               copyOnly = true;
            }
         }

         if (copyOnly)
         {
            File.Copy(oldPath, newPath);
         }

         RotateImages(newPath);
      }

      private static void RotateImages(string destination)
      {
         var filePath = destination;

         File.SetAttributes(filePath, FileAttributes.Normal);

         var rotated = false;
         var fileNameTemp = Path.Combine(Path.GetDirectoryName(filePath), Path.GetRandomFileName());

         using (var image = new Bitmap(filePath))
         {

            Encoder enc = Encoder.Transformation;
            var encParms = new EncoderParameters(1);
            ImageCodecInfo codecInfo = GetEncoderInfo("image/jpeg");

            foreach (PropertyItem item in image.PropertyItems)
            {
               if (item.Id == 274)
               {
                  EncoderParameter encParm;
                  switch (item.Value[0])
                  {
                     case 1:
                        break;
                     case 6:
                        rotated = true;
                        encParm = new EncoderParameter(enc, (long)EncoderValue.TransformRotate90);
                        encParms.Param[0] = encParm;
                        item.Value[0] = 1;
                        image.RemovePropertyItem(item.Id);
                        image.SetPropertyItem(item);
                        image.Save(fileNameTemp, codecInfo, encParms);
                        break;
                     case 8:
                        rotated = true;
                        encParm = new EncoderParameter(enc, (long)EncoderValue.TransformRotate270);
                        encParms.Param[0] = encParm;
                        item.Value[0] = 1;
                        image.RemovePropertyItem(item.Id);
                        image.SetPropertyItem(item);
                        image.Save(fileNameTemp, codecInfo, encParms);
                        break;
                  }
               }
            }
         }

         if (rotated)
         {
            File.Replace(fileNameTemp, filePath, null);
         }
      }

      private static ImageCodecInfo GetEncoderInfo(String mimeType)
      {
         int j;
         ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
         for (j = 0; j < encoders.Length; ++j)
         {
            if (encoders[j].MimeType == mimeType)
               return encoders[j];
         }
         return null;
      }

   }
}
