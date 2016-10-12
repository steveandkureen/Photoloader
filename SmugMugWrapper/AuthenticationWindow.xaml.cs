using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmugMugWrapper
{
   /// <summary>
   /// Interaction logic for AuthenticationWindow.xaml
   /// </summary>
   public partial class AuthenticationWindow : Window
   {
      private string authUrl;

      public AuthenticationWindow(string url)
      {
         authUrl = url;
         InitializeComponent();
         Browser.Address = authUrl;
         
         //Browser.WebBrowser.Load(authUrl);
      }



      public string AccessPin { get; set; }

      private void WindowLoaded(object sender, RoutedEventArgs e)
      {
         
      }

      private void DoneClick(object sender, RoutedEventArgs e)
      {
         Close();
      }
   }
}
