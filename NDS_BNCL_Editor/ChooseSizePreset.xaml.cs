using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace NDS_BNCL_Editor
{
    /// <summary>
    /// Interaction logic for ChooseSizePreset.xaml
    /// </summary>
    public partial class ChooseSizePreset : Window
    {
        public static bool Complete = false;
        XDocument SizePresetsXML;

        public ChooseSizePreset()
        {
            Complete = false;
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SizePresetsXML = XDocument.Load("SizePresets.xml");
                var GameNames = SizePresetsXML.Descendants("Game").Attributes("Name");
                foreach (var GameName in GameNames)
                {
                    game_ComboBox.Items.Add(GameName.Value);
                }
            }
            catch (Exception exception)
            {
                new ErrorHandler(exception, "XML Parse Failed!", "Error!").ShowDialog();
                Close();
            }
        }

        async private void Game_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(1);

            scene_ComboBox.Items.Clear();

            var SceneNames = SizePresetsXML
                .Descendants("Game")
                .Where(node => (string)node.Attribute("Name") == game_ComboBox.Text)
                .Descendants("Scene")
                .Attributes("Name");
            foreach (var SceneName in SceneNames)
            {
                scene_ComboBox.Items.Add(SceneName.Value);
            }
        }

        async private void Scene_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(1);
            
            for (int i = 0; i < 256; i++)
            {
                MainWindow.graphicID_width[i] = 0;
                MainWindow.graphicID_height[i] = 0;
                MainWindow.graphicID_comment[i] = null;
                try
                {
                    var GraphicsWidth = SizePresetsXML
                        .Descendants("Game")
                        .Where(node => (string)node.Attribute("Name") == game_ComboBox.Text)
                        .Descendants("Scene")
                        .Where(node => (string)node.Attribute("Name") == scene_ComboBox.Text)
                        .Descendants("Graphic")
                        .Where(node => (int)node.Attribute("ID") == i)
                        .Attributes("Width");
                    foreach (var GraphicWidth in GraphicsWidth)
                    {
                        MainWindow.graphicID_width[i] = byte.Parse(GraphicWidth.Value);
                    }

                    var GraphicsHeight = SizePresetsXML
                        .Descendants("Game")
                        .Where(node => (string)node.Attribute("Name") == game_ComboBox.Text)
                        .Descendants("Scene")
                        .Where(node => (string)node.Attribute("Name") == scene_ComboBox.Text)
                        .Descendants("Graphic")
                        .Where(node => (int)node.Attribute("ID") == i)
                        .Attributes("Height");
                    foreach (var GraphicHeight in GraphicsHeight)
                    {
                        MainWindow.graphicID_height[i] = byte.Parse(GraphicHeight.Value);
                    }

                    var GraphicsComment = SizePresetsXML
                        .Descendants("Game")
                        .Where(node => (string)node.Attribute("Name") == game_ComboBox.Text)
                        .Descendants("Scene")
                        .Where(node => (string)node.Attribute("Name") == scene_ComboBox.Text)
                        .Descendants("Graphic")
                        .Where(node => (int)node.Attribute("ID") == i)
                        .Attributes("Comment");
                    foreach (var GraphicComment in GraphicsComment)
                    {
                        MainWindow.graphicID_comment[i] = GraphicComment.Value;
                    }
                }
                catch(Exception exception)
                {
                    new ErrorHandler(exception, "XML Parse Failed!", "Error!").ShowDialog();
                    Close();
                }
            }
        }

        private void Done_Button_Click(object sender, RoutedEventArgs e)
        {
            if (game_ComboBox.Text != "" && scene_ComboBox.Text != "")
            {
                Complete = true;
                Close();
            }
            else
                MessageBox.Show("No preset was selected!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            Complete = false;
            Close();
        }
    }
}
