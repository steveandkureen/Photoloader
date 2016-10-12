using System.IO;
using System.Windows;
using System.Xml.Serialization;
using SmugMugWrapper;

namespace PhotoLoader4
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        readonly string settingsFilePath;

        public SettingsWindow(string path)
        {
            InitializeComponent();

            settingsFilePath = path;
            Settings = LoadSettings(settingsFilePath);
            DataContext = Settings;
        }

        private void OkButtonClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        public static PhotoLoaderSettings LoadSettings(string path)
        {
            PhotoLoaderSettings settings;
            var serializer = new XmlSerializer(typeof(PhotoLoaderSettings));
            if (!File.Exists(path))
            {
                var pathDir = Path.GetDirectoryName(path);
                if (pathDir != null && !Directory.Exists(pathDir))
                {
                    Directory.CreateDirectory(pathDir);
                }

                settings = new PhotoLoaderSettings();
                using (var stream = File.Create(path))
                {
                    serializer.Serialize(stream, settings);
                }
            }

            using (var readStream = File.OpenRead(path))
            {
                settings = (PhotoLoaderSettings)serializer.Deserialize(readStream);
            }

            return settings;
        }


        void SaveSettings()
        {
            var serializer = new XmlSerializer(typeof(PhotoLoaderSettings));
            using (var stream = File.CreateText(settingsFilePath))
            {
                serializer.Serialize(stream, Settings);
            }
        }


        public PhotoLoaderSettings Settings { get; set; }

        private void RegisterClientClick(object sender, RoutedEventArgs e)
        {
            var token = new OAuthToken();
            var smugApi =
               new SmugMugWrapper.SmugMugApiV2(token);

            token = smugApi.GetAccessToken();

            var settingsPath = Path.Combine(App.Current.ApplicationDataFolder, "SmugMug.dat");
            using (var writer = File.Open(settingsPath, FileMode.OpenOrCreate))
            {
                var serializer = new XmlSerializer(typeof(OAuthToken));
                serializer.Serialize(writer, token);
            }
        }
    }
}
