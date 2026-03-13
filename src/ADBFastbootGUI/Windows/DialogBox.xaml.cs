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
using System.Windows.Media.Animation;
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
            var sb = new Storyboard();

            var opacityAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var topAnim = new DoubleAnimation(this.Top, this.Top - 20, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            sb.Children.Add(opacityAnim);
            sb.Children.Add(topAnim);

            Storyboard.SetTarget(opacityAnim, this);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

            Storyboard.SetTarget(topAnim, this);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

            sb.Completed += (s, _) =>
            {
                Application.Current.MainWindow.Opacity = 1;
                this.Close();
            };
            sb.Begin();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            var sb = new Storyboard();

            var opacityAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var topAnim = new DoubleAnimation(this.Top, this.Top - 20, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            sb.Children.Add(opacityAnim);
            sb.Children.Add(topAnim);

            Storyboard.SetTarget(opacityAnim, this);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

            Storyboard.SetTarget(topAnim, this);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

            sb.Completed += (s, _) =>
            {
                Application.Current.MainWindow.Opacity = 1;
                this.Close();
            };
            sb.Begin();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            Application.Current.MainWindow.Opacity = 1.0;
            var sb = new Storyboard();

            var opacityAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            var topAnim = new DoubleAnimation(this.Top, this.Top - 20, TimeSpan.FromMilliseconds(200))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };

            sb.Children.Add(opacityAnim);
            sb.Children.Add(topAnim);

            Storyboard.SetTarget(opacityAnim, this);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

            Storyboard.SetTarget(topAnim, this);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

            sb.Completed += (s, _) =>
            {
                Application.Current.MainWindow.Opacity = 1;
                this.Close();
            };
            sb.Begin();
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Opacity = 0;
            this.Top -= 20;

            var opacityAnim = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var topAnim = new DoubleAnimation(this.Top, this.Top + 20, TimeSpan.FromMilliseconds(300))
            {
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            var sb = new Storyboard();
            sb.Children.Add(opacityAnim);
            sb.Children.Add(topAnim);

            Storyboard.SetTarget(opacityAnim, this);
            Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

            Storyboard.SetTarget(topAnim, this);
            Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

            sb.Begin();
        }
    }
}
