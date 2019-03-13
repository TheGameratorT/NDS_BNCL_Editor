using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NDS_BNCL_Editor
{
    /// <summary>
    /// Interaction logic for ErrorHandler.xaml
    /// </summary>
    public partial class ErrorHandler : Window
    {
        public ErrorHandler(Exception exception, string message = "An unhandled exception has occurred!", string title = "Error!", string errorIcon = "Resources/user32_103.ico")
        {
            InitializeComponent();

            ExceptionBox.Text = exception.ToString();
            MessageLabel.Content = message;
            Title = title;
            ErrorIconImage.Source = new BitmapImage(new Uri(errorIcon, UriKind.RelativeOrAbsolute));
        }
    }
}
