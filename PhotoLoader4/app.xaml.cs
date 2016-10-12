using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;

namespace PhotoLoader4
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      public static new App Current { get; set; }


      
      private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
      {
         Current = this;
      }

      public string ApplicationDataFolder
      {
         get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PhotoLoader"); }
      }

      private void Application_Startup(object sender, StartupEventArgs e)
      {
         Current = this;
      }
      
   }

}
