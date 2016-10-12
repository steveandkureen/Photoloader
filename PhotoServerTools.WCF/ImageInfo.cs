using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace PhotoServerTools
{
   [TypeConverter(typeof(InfoTypeConverter))]
   [DataContract]
   public class ImageInfo
   {
      [DataMember]
      public string Caption { get; set; }

      [DataMember]
      public bool IsPrivate { get; set; }

      public ImageInfo(string caption, bool isPrivate)
      {
         IsPrivate = isPrivate;
         Caption = caption;
      }
   }

   public class InfoTypeConverter : TypeConverter
   {
      private static readonly Regex ImageInfoRegex = new Regex("\\[(\".*\"),(\".*\")\\]");

      public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
      {
         return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
      }

      public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
      {
         var strValue = value as string;
         if (strValue != null)
         {
            var match = ImageInfoRegex.Match(strValue);
            if (match.Success)
            {
               return new ImageInfo(match.Groups[0].Value, Convert.ToBoolean(match.Groups[1].Value));
            }
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
            var imageInfo = (ImageInfo)value;
            return string.Format("[{0},{1}]", imageInfo.Caption, imageInfo.IsPrivate);
         }
         return base.ConvertTo(context, culture, value, destinationType);
      }
   }
}
