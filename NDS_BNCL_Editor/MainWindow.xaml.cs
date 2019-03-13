using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NDS_BNCL_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Button[] objn_button = new Button[256];
        bool UpdaterCanCreateButtons = true;
        ushort[] objn_xPos = new ushort[256];
        ushort[] objn_yPos = new ushort[256];
        byte[] objn_xAlign = new byte[256];
        byte[] objn_yAlign = new byte[256];
        byte[] objn_graphicID = new byte[256];
        public static byte[] graphicID_width = new byte[256];
        public static byte[] graphicID_height = new byte[256];
        public static string[] graphicID_comment = new string[256];

        public MainWindow()
        {
            InitializeComponent();

            if (App.ExeArgs.Length > 0)
            {
                loadBNCLfile(App.ExeArgs[0]);
            }

            ViewerMode_ComboBox.SelectedIndex = 0;
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "BNCL files|*.bncl";
            if (openFileDialog.ShowDialog() == true)
                loadBNCLfile(openFileDialog.FileName);
        }

        private void loadBNCLfile(string BNCLpath)
        {
            UpdaterCanCreateButtons = false;

            numberOfObjs_UpDown.Value = 0;
            for (int i = 1; i < objn_button.Length; i++)
            {
                WindowGrid.Children.Remove(objn_button[i]);
                objn_button[i] = null;
                objn_xPos[i] = 0;
                objn_yPos[i] = 0;
                objn_xAlign[i] = 0;
                objn_yAlign[i] = 0;
                objn_graphicID[i] = 0;
                graphicID_width[i] = 0;
                graphicID_height[i] = 0;
                graphicID_comment[i] = "";
            }

            BinaryReader openedFileData_reader = new BinaryReader(File.Open(BNCLpath, FileMode.Open));

            if (Encoding.ASCII.GetString(openedFileData_reader.ReadBytes(4)) != "JNCL")
            {
                MessageBoxResult loadanyways = MessageBox.Show("The header of this BNCD file is invalid!\n\nWould you like to load it anyways?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (loadanyways == MessageBoxResult.No)
                {
                    openedFileData_reader.Close();
                    return;
                }
            }

            new ChooseSizePreset().ShowDialog();
            if(!ChooseSizePreset.Complete)
            {
                openedFileData_reader.Close();
                UpdaterCanCreateButtons = true;
                return;
            }

            openedFileData_reader.BaseStream.Position = 0x6;
            numberOfObjs_UpDown.Value = openedFileData_reader.ReadByte();

            for (int i = 1; i <= numberOfObjs_UpDown.Value; i++)
            {
                objn_button[i] = new Button()
                {
                    Opacity = 0.75,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = string.Format("Object {0}", i),
                    Name = string.Format("ObjectN_Button_{0}", i),
                    Tag = i
                };
                objn_button[i].Click += new RoutedEventHandler(objn_Click);
                WindowGrid.Children.Add(objn_button[i]);

                openedFileData_reader.BaseStream.Position = 0x8 - 0x8 + (i * 0x8);
                ushort xPosUInt16 = openedFileData_reader.ReadUInt16();
                objn_xAlign[i] = (byte)(xPosUInt16 >> 12 & 3);
                objn_xPos[i] = (ushort)(xPosUInt16 & 0xFFF);
                if (objn_xPos[i] > 255)
                    objn_xPos[i] = 255;

                openedFileData_reader.BaseStream.Position = 0xA - 0x8 + (i * 0x8);
                ushort yPosUInt16 = openedFileData_reader.ReadUInt16();
                objn_yAlign[i] = (byte)(yPosUInt16 >> 12 & 3);
                objn_yPos[i] = (ushort)(yPosUInt16 & 0xFFF);
                if (objn_yPos[i] > 255)
                    objn_yPos[i] = 255;

                openedFileData_reader.BaseStream.Position = 0xC - 0x8 + (i * 0x8);
                objn_graphicID[i] = openedFileData_reader.ReadByte();

                /*int xPosRenderResult = objn_xPos[i];
                if (objn_xAlign[i] == 1)
                    xPosRenderResult -= (byte)((graphicID_width[objn_graphicID[i]] + 1) / 2);
                if (objn_xAlign[i] == 2)
                    xPosRenderResult -= graphicID_width[objn_graphicID[i]];

                int yPosRenderResult = objn_yPos[i];
                if (objn_yAlign[i] == 1)
                    yPosRenderResult -= (byte)((graphicID_height[objn_graphicID[i]] + 1) / 2);
                if (objn_yAlign[i] == 2)
                    yPosRenderResult -= graphicID_height[objn_graphicID[i]];

                objn_button[i].Margin = new Thickness(xPosRenderResult + guideImage.Margin.Left, yPosRenderResult + guideImage.Margin.Top, 0, 0);
                objn_button[i].Width = graphicID_width[objn_graphicID[i]];
                objn_button[i].Height = graphicID_height[objn_graphicID[i]];*/
            }
            
            Console.WriteLine("Number of Graphic IDs: " + objn_graphicID.Max());

            xPos_UpDown.Value = objn_xPos[1];
            yPos_UpDown.Value = objn_yPos[1];
            xPosAlign_UpDown.Value = objn_xAlign[1];
            yPosAlign_UpDown.Value = objn_yAlign[1];
            graphicID_UpDown.Value = objn_graphicID[1];
            CommentLabel.Content = graphicID_comment[objn_graphicID[1]];
            objn_button[1].Background = Brushes.Yellow;

            openedFileData_reader.Close();
            currentObj_UpDown.Value = 1;
            UpdaterCanCreateButtons = true;
        }

        private void objn_Click(object sender, RoutedEventArgs e)
        {
            currentObj_UpDown.Value = (int)(sender as Button).Tag;
        }

        private void openImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "All supported image formats|*bmp; *gif; *.jpg; *.jpeg; *.jpe; *.jif; *.jfif; *.jfi; *png; *.tiff; *.tif|" +
                "Bitmap images|*.bmp|" +
                "GIF images|*.gif|" +
                "JPEG images|*.jpg; *.jpeg; *.jpe; *.jif; *.jfif; *.jfi|" +
                "PNG images|*.png|" +
                "TIFF images|*.tiff; *.tif|" +
                "All files|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage bmpImg = new BitmapImage();
                bmpImg.BeginInit();
                bmpImg.UriSource = new Uri(openFileDialog.FileName);
                bmpImg.EndInit();
                guideImage.Source = bmpImg;
            }
        }

        private void OnSelectedObjectChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (currentObj_UpDown != null &&
                numberOfObjs_UpDown != null &&
                xPos_UpDown != null &&
                yPos_UpDown != null &&
                xPosAlign_UpDown != null &&
                yPosAlign_UpDown != null &&
                graphicID_UpDown != null &&
                CommentLabel != null)
            {
                currentObj_UpDown.Maximum = numberOfObjs_UpDown.Value;
                if (currentObj_UpDown.Value > currentObj_UpDown.Maximum && currentObj_UpDown.Value != 1)
                {
                    currentObj_UpDown.Value = currentObj_UpDown.Maximum;
                }

                xPos_UpDown.Value = objn_xPos[(byte)currentObj_UpDown.Value];
                yPos_UpDown.Value = objn_yPos[(byte)currentObj_UpDown.Value];
                xPosAlign_UpDown.Value = objn_xAlign[(byte)currentObj_UpDown.Value];
                yPosAlign_UpDown.Value = objn_yAlign[(byte)currentObj_UpDown.Value];
                graphicID_UpDown.Value = objn_graphicID[(byte)currentObj_UpDown.Value];
                CommentLabel.Content = graphicID_comment[(byte)graphicID_UpDown.Value];
            }

            for (int i = 1; i < objn_button.Length; i++)
            {
                if (objn_button[i] != null)
                {
                    if (currentObj_UpDown.Value == i)
                        objn_button[i].Background = Brushes.Yellow;
                    else
                        objn_button[i].Background = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));

                    if (i > numberOfObjs_UpDown.Value)
                    {
                        WindowGrid.Children.Remove(objn_button[i]);
                        objn_button[i] = null;
                    }
                }
                else if (objn_button[i] == null && numberOfObjs_UpDown.Value >= i)
                {
                    if (UpdaterCanCreateButtons == true)
                    {
                        objn_button[i] = new Button()
                        {
                            Margin = new Thickness(guideImage.Margin.Left, guideImage.Margin.Top, 0, 0),
                            Opacity = 0.75,
                            VerticalAlignment = VerticalAlignment.Top,
                            HorizontalAlignment = HorizontalAlignment.Left,
                            Content = string.Format("Object {0}", i),
                            Name = string.Format("ObjectN_Button_{0}", i),
                            Tag = i
                        };
                        objn_button[i].Click += new RoutedEventHandler(objn_Click);
                        WindowGrid.Children.Add(objn_button[i]);

                        objn_xPos[i] = 0;
                        objn_xAlign[i] = 0;
                        objn_yPos[i] = 0;
                        objn_yAlign[i] = 0;
                        objn_graphicID[i] = 0;
                    }
                }
            }
        }

        async public void OnObjectPropertiesChange(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            await Task.Delay(1);

            byte currentObject = (byte)currentObj_UpDown.Value;
            if (objn_button[currentObject] != null &&
                xPos_UpDown != null &&
                yPos_UpDown != null &&
                xPosAlign_UpDown != null &&
                yPosAlign_UpDown != null &&
                graphicID_UpDown != null &&
                CommentLabel != null)
            {
                objn_xPos[currentObject] = (ushort)xPos_UpDown.Value;
                objn_yPos[currentObject] = (ushort)yPos_UpDown.Value;
                objn_xAlign[currentObject] = (byte)xPosAlign_UpDown.Value;
                objn_yAlign[currentObject] = (byte)yPosAlign_UpDown.Value;
                objn_graphicID[currentObject] = (byte)graphicID_UpDown.Value;

                RenderView();
            }
        }

        private void RenderView()
        {
            for(int i = 1; i < objn_button.Length; i++)
            {
                if (objn_button[i] != null)
                {
                    int xPosRenderResult = objn_xPos[i];
                    if (objn_xAlign[i] == 1)
                        xPosRenderResult -= (byte)((graphicID_width[objn_graphicID[i]] + 1) / 2);
                    if (objn_xAlign[i] == 2)
                        xPosRenderResult -= graphicID_width[objn_graphicID[i]];

                    int yPosRenderResult = objn_yPos[i];
                    if (objn_yAlign[i] == 1)
                        yPosRenderResult -= (byte)((graphicID_height[objn_graphicID[i]] + 1) / 2);
                    if (objn_yAlign[i] == 2)
                        yPosRenderResult -= graphicID_height[objn_graphicID[i]];

                    objn_button[i].Margin = new Thickness(xPosRenderResult + guideImage.Margin.Left, yPosRenderResult + guideImage.Margin.Top, 0, 0);
                    objn_button[i].Width = graphicID_width[objn_graphicID[i]];
                    objn_button[i].Height = graphicID_height[objn_graphicID[i]];

                    if (graphicID_width[objn_graphicID[i]] <= 48)
                    {
                        objn_button[i].Content = i;
                        objn_button[i].FontSize = 10;
                    }
                    else if (graphicID_width[objn_graphicID[i]] > 48)
                    {
                        objn_button[i].Content = string.Format("Object {0}", i);
                        objn_button[i].FontSize = 12;
                    }
                }
            }
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "BNCL files|*.bncl";
            if (saveFileDialog.ShowDialog() == true)
                saveBNCLfile(saveFileDialog.FileName);
        }

        private void saveBNCLfile(string BNCLpath)
        {
            BinaryWriter saveFileFromDialog_writer = new BinaryWriter(File.Open(BNCLpath, FileMode.Create));

            saveFileFromDialog_writer.Write("JNCL".ToCharArray());

            saveFileFromDialog_writer.BaseStream.Position = 0x6;
            saveFileFromDialog_writer.BaseStream.WriteByte((byte)numberOfObjs_UpDown.Value);

            if (numberOfObjs_UpDown.Value == 0)
            {
                saveFileFromDialog_writer.Close();
                return;
            }

            for (int i = 1; i <= numberOfObjs_UpDown.Value; i++)
            {
                saveFileFromDialog_writer.BaseStream.Position = 0x8 - 0x8 + (i * 0x8);
                if (objn_xAlign[i] == 0)
                    saveFileFromDialog_writer.Write(objn_xPos[i]);
                else if (objn_xAlign[i] == 1)
                    saveFileFromDialog_writer.Write((ushort)(objn_xPos[i] + 0x1000));
                else if (objn_xAlign[i] == 2)
                    saveFileFromDialog_writer.Write((ushort)(objn_xPos[i] + 0x2000));

                saveFileFromDialog_writer.BaseStream.Position = 0xA - 0x8 + (i * 0x8);
                if (objn_yAlign[i] == 0)
                    saveFileFromDialog_writer.Write(objn_yPos[i]);
                else if (objn_yAlign[i] == 1)
                    saveFileFromDialog_writer.Write((ushort)(objn_yPos[i] + 0x1000));
                else if (objn_yAlign[i] == 2)
                    saveFileFromDialog_writer.Write((ushort)(objn_yPos[i] + 0x2000));

                saveFileFromDialog_writer.BaseStream.Position = 0xC - 0x8 + (i * 0x8);
                saveFileFromDialog_writer.BaseStream.WriteByte(objn_graphicID[i]);
            }

            saveFileFromDialog_writer.Close();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            new SettingsWindow().ShowDialog();
        }

        private void GfxIDsettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            new gfxIDsettingsWindow().ShowDialog();
            RenderView();
        }

        async private void ViewerMode_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await Task.Delay(1);

            if (ViewerMode_ComboBox.SelectedIndex == 0)
            {
                Application.Current.MainWindow.Width = 535;
                Application.Current.MainWindow.Height = 350;
                openImgBtn.Margin = new Thickness(440, 10, 0, 0);
                settingsBtn.Margin = new Thickness(375, 10, 0, 0);
                HiddenRedRectangle.Visibility = Visibility.Collapsed;
                guideImage.Margin = new Thickness(264, 119, 0, 0);
                guideImageLayout.Margin = new Thickness(264, 119, 0, 0);

                RenderView();
            }

            if (ViewerMode_ComboBox.SelectedIndex == 1)
            {
                Application.Current.MainWindow.Width = 1045;
                Application.Current.MainWindow.Height = 730;
                openImgBtn.Margin = new Thickness(948, 10, 0, 0);
                settingsBtn.Margin = new Thickness(883, 10, 0, 0);
                HiddenRedRectangle.Visibility = Visibility.Visible;
                guideImage.Margin = new Thickness(HiddenRedRectangle.Margin.Left + 255, HiddenRedRectangle.Margin.Top + 192, 0, 0);
                guideImageLayout.Margin = new Thickness(HiddenRedRectangle.Margin.Left + 255, HiddenRedRectangle.Margin.Top + 192, 0, 0);

                RenderView();
            }
        }
    }
}