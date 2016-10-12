using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace PhotoServerTools
{
   [TypeConverter(typeof(PathTypeConverter))]
   [DataContract]
   public class ImagePath
   {
      [DataMember]
       public string Path;

      public string RootPath { get; set; }

      const string ThumbsRoot = "Thumbs";
      const string CacheRoot = "Cache";
      const string PhotoRoot = "Photos";

      public ImagePath(string path, string rootPath = "")
      {
         Path = path.TrimStart(new char[] { '/', '\\' });
         RootPath = rootPath;
      }
      
      public bool IsImage {
            get
              {
                  var extension = System.IO.Path.GetExtension(Path);
                  return extension != null && extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase);
              }
      }

      public string GetThumbPath(bool isUrl, bool isRelative = true)
      {
         var path = isRelative ? ThumbsRoot : System.IO.Path.Combine(RootPath, ThumbsRoot);

          var directoryName = System.IO.Path.GetDirectoryName(path);
          if (isUrl)
         {
            return System.IO.Path.Combine(System.IO.Path.Combine(path, directoryName), System.IO.Path.GetFileNameWithoutExtension(path) + "_thmb" + System.IO.Path.GetExtension(path)).Replace('\\', '/').Replace("~", "%7E");
         }
         return System.IO.Path.Combine(System.IO.Path.Combine(path, directoryName), System.IO.Path.GetFileNameWithoutExtension(path) + "_thmb" + System.IO.Path.GetExtension(path)).Replace('/', '\\');
      }

      public string GetCachePath(bool isUrl, bool isRelative = true)
      {
         var path = isRelative ? CacheRoot : System.IO.Path.Combine(RootPath, CacheRoot);

         if (isUrl)
         {
            return System.IO.Path.Combine(System.IO.Path.Combine(path, System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileNameWithoutExtension(path) + "_cache" + System.IO.Path.GetExtension(path)).Replace('\\', '/').Replace("~", "%7E");
         }
         return System.IO.Path.Combine(System.IO.Path.Combine(path, System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileNameWithoutExtension(path) + "_cache" + System.IO.Path.GetExtension(path)).Replace('/', '\\');
      }

      public string GetPhotoPath(bool isUrl, bool isRelative = true)
      {
         var path = isRelative ? PhotoRoot : System.IO.Path.Combine(RootPath, PhotoRoot);

         if (isUrl)
         {
            return System.IO.Path.Combine(System.IO.Path.Combine(path, System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileNameWithoutExtension(path) + System.IO.Path.GetExtension(path)).Replace('\\', '/').Replace("~","%7E");
         }
         return System.IO.Path.Combine(System.IO.Path.Combine(path, System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileNameWithoutExtension(path) + System.IO.Path.GetExtension(path)).Replace('/', '\\');
      }

      public string GetRawPath(bool isRelative = true)
      {
         return Path.Replace('/', '\\');
      }

      public string GetRawUrlPath(bool isRelative = true)
      {
         return Path.Replace('\\', '/');
      }

   }

   public class PathTypeConverter : TypeConverter
   {
      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         return sourceType == typeof (string) || base.CanConvertFrom(context, sourceType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
      {
         var strValue = value as string;
         if (strValue != null)
         {
            return new ImagePath(strValue.Trim(new[] {'[', ']'}));
         }
         return base.ConvertFrom(context, culture, value);
      }

      public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
      {
         return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
      }

      public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
      {
         if (destinationType == typeof(string))
         {
            var imagePath = (ImagePath) value;
            return string.Format("[{0}]", imagePath.Path);
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }
}
