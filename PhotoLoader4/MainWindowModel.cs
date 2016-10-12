using System.Collections.Generic;
using PhotoServerTools;

namespace PhotoLoader4
{
   public class MainWindowModel
   {
      public bool IsFtp { get; set; }
      public bool CopyOnly { get; set; }
      public PhotoLoaderSettings Settings { get; set; }

      public List<PhotoInfo> PhotoList { get; set; }
   }
}
