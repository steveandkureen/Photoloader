using System.Collections.Generic;

namespace PhotoLoader4
{
   public class PhotoLoaderSettings
   {
      public PhotoLoaderSettings()
      {
         SourcePaths = @"C:\temp\SourceFolder";
         DestPath = @"C:\temp\DestFolder";
      }

      public List<string> ImageTypes { get; set; }

      public List<string> VideoTypes { get; set; }

      public string SourcePaths { get; set; }

      public string DestPath { get; set; }

   }
}
