using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NDS_BNCL_Editor
{
    /// <summary>
    /// Interaction logic for gfxIDsettingsWindow.xaml
    /// </summary>
    public partial class gfxIDsettingsWindow : Window
    {
        public gfxIDsettingsWindow()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;

            width_UpDown.Value = MainWindow.graphicID_width[(byte)currentGfxID.Value];
            height_UpDown.Value = MainWindow.graphicID_height[(byte)currentGfxID.Value];
            commentBox.Text = MainWindow.graphicID_comment[(byte)currentGfxID.Value];
        }

        private void Note_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This changes won't take effect in game as those options are harcoded and these properties are impossible to save in the BNCL file! This menu was created in order for the 2D Viewer to work.\n\nCuriosity: The graphic IDs were just a reference for the Nintendo developers being able to see the graphic in their editors.");
        }

        async private void CurrentGfxID_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            await Task.Delay(1);
            width_UpDown.Value = MainWindow.graphicID_width[(byte)currentGfxID.Value];
            height_UpDown.Value = MainWindow.graphicID_height[(byte)currentGfxID.Value];
            commentBox.Text = MainWindow.graphicID_comment[(byte)currentGfxID.Value];
        }

        async private void OnGFXIDPropertyChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            await Task.Delay(1);
            MainWindow.graphicID_width[(byte)currentGfxID.Value] = (byte)width_UpDown.Value;
            MainWindow.graphicID_height[(byte)currentGfxID.Value] = (byte)height_UpDown.Value;
        }

        async private void CommentBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            await Task.Delay(1);
            MainWindow.graphicID_comment[(byte)currentGfxID.Value] = commentBox.Text;
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BNCL Graphic ID Setting Files|*.bnclgp";
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (BinaryWriter saveFileFromDialog_writer = new BinaryWriter(File.Open(saveFileDialog.FileName, FileMode.Create)))
                    {
                        saveFileFromDialog_writer.Write("BNCLGP".ToCharArray());

                        for (int i = 0; i < 255; i++)
                        {
                            saveFileFromDialog_writer.BaseStream.Position = i * 32 + 16;
                            
                            saveFileFromDialog_writer.Write(MainWindow.graphicID_width[i]);
                            saveFileFromDialog_writer.Write(MainWindow.graphicID_height[i]);
                            if (MainWindow.graphicID_comment[i] != null)
                                saveFileFromDialog_writer.Write(MainWindow.graphicID_comment[i]);
                        }
                    }
                }
                catch(Exception exception)
                {
                    new ErrorHandler(exception, "Error saving file!").ShowDialog();
                }
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BNCL Graphic ID Setting Files|*.bnclgp";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (BinaryReader openFileFromDialog_reader = new BinaryReader(File.Open(openFileDialog.FileName, FileMode.Open)))
                    {
                        if (openFileFromDialog_reader.ReadChars(4) == "BNCLGP".ToCharArray())
                        {
                            MessageBoxResult loadanyways = MessageBox.Show("The header of this BNCLGP file is invalid!\n\nWould you like to load it anyways?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                            if (loadanyways == MessageBoxResult.No)
                                return;
                        }

                        for (int i = 0; i < 255; i++)
                        {
                            openFileFromDialog_reader.BaseStream.Position = i * 32 + 16;

                            MainWindow.graphicID_width[i] = openFileFromDialog_reader.ReadByte();
                            MainWindow.graphicID_height[i] = openFileFromDialog_reader.ReadByte();
                            MainWindow.graphicID_comment[i] = openFileFromDialog_reader.ReadString();
                        }

                        width_UpDown.Value = MainWindow.graphicID_width[(byte)currentGfxID.Value];
                        height_UpDown.Value = MainWindow.graphicID_height[(byte)currentGfxID.Value];
                        commentBox.Text = MainWindow.graphicID_comment[(byte)currentGfxID.Value];
                    }
                }
                catch (Exception exception)
                {
                    new ErrorHandler(exception, "Error saving file!").ShowDialog();
                }
            }
        }
    }
}
