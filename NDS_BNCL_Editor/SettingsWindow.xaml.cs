using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NDS_BNCL_Editor
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
        }

        private void Credits_Click(object sender, RoutedEventArgs e)
        {
            new Credits().ShowDialog();
        }

        private void UpdateSizePresets_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile("https://raw.githubusercontent.com/TheGameratorT/NDS_BNCL_Editor/master/NDS_BNCL_Editor/SizePresets.xml", "SizePresets_NEW.xml");
            }

            if(new FileInfo("SizePresets_NEW.xml").Length == 0)
            {
                MessageBox.Show("The download has failed because we could not contact the download server!\nPlease check your internet connection and try again.");
                File.Delete("SizePresets_NEW.xml");
                return;
            }

            if(!File.Exists("SizePresets.xml"))
            {
                File.Copy("SizePresets_NEW.xml", "SizePresets.xml", true);
                File.Delete("SizePresets_NEW.xml");
                MessageBox.Show("SizePresets.xml was successfully updated!");
            }
            else if (File.ReadAllLines("SizePresets.xml") != File.ReadAllLines("SizePresets_NEW.xml"))
            {
                File.Copy("SizePresets_NEW.xml", "SizePresets.xml", true);
                File.Delete("SizePresets_NEW.xml");
                MessageBox.Show("SizePresets.xml was successfully updated!");
            }
            else
            {
                MessageBox.Show("SizePresets.xml is already up to date!");
            }
        }

        private void ChooseSizePreset_Click(object sender, RoutedEventArgs e)
        {
            new ChooseSizePreset().ShowDialog();
        }
    }
}
