using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ADBFastbootGUI.Windows
{
    /// <summary>
    /// Interaction logic for DialogBox.xaml
    /// </summary>
    public enum DialogBoxButton
    {
        OK,
        OKCancel,
        YesNo,
        YesCancel
    }
    public enum DialogBoxSize
    {
        Manual = 0,
        //
        // Summary:
        //     Specifies that a window will automatically set its width to fit the width of
        //     its content, but not the height.
        Width = 1,
        //
        // Summary:
        //     Specifies that a window will automatically set its height to fit the height of
        //     its content, but not the width.
        Height = 2,
        //
        // Summary:
        //     Specifies that a window will automatically set both its width and height to fit
        //     the width and height of its content.
        WidthAndHeight = 3
    }

    public partial class DialogBox : Window
    {
        private DialogBox(string message, DialogBoxButton buttons)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            Message.Text = message;

            switch (buttons)
            {
                case DialogBoxButton.OK:
                    CancelButton.Visibility = Visibility.Collapsed;
                    break;
                case DialogBoxButton.OKCancel:
                    break;
                case DialogBoxButton.YesNo:
                    OKButton.Content = "Yes"; 
                    CancelButton.Content = "No";
                    break;
                case DialogBoxButton.YesCancel:
                    OKButton.Content = "Yes";
                    CancelButton.Content = "Cancel";
                    break;

            }
        }

        #region Statik 'Show' Metotları (MessageBox gibi kullanmak için)

        public static bool Show(string message)
        {
            try
            {
                var dialog = new DialogBox(message, DialogBoxButton.OK);
                Application.Current.MainWindow.Opacity = 0.4;
                dialog.ShowDialog();
                Application.Current.MainWindow.Opacity = 0.4;
                return true;
            }
            finally
            {
                Application.Current.MainWindow.Opacity = 1;
            }
        }

        public static bool Show(string message, DialogBoxButton buttons)
        {
            var dialog = new DialogBox(message, buttons);
            try
            {
                Application.Current.MainWindow.Opacity = 0.4;
                return dialog.ShowDialog() ?? false;
            }
            finally
            {
                Application.Current.MainWindow.Opacity = 1;
            }
        }
        public static bool Show(string message, DialogBoxButton buttons, DialogBoxSize size)
        {
            var dialog = new DialogBox(message, buttons);
            try
            {
                dialog.SizeToContent = (SizeToContent)size;
                Application.Current.MainWindow.Opacity = 0.4;
                return dialog.ShowDialog() ?? false;
            }
            finally
            {
                Application.Current.MainWindow.Opacity = 1;
            }
        }

        #endregion

        #region Pencere Olayları (Event Handlers)

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Application.Current.MainWindow.Opacity = 1.0;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Application.Current.MainWindow.Opacity = 1.0;
            this.Close();
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #endregion
    }
}
