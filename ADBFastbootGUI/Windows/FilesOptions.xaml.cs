using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for FilesOptions.xaml
    /// </summary>
    public partial class FilesOptions : Window
    {
        MainWindow mw = new MainWindow();
        string adbpath = $@"C:\Program Files\ADBFastbootGUI\";
        public FilesOptions()
        {
            InitializeComponent();
        }
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
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

            sb.Completed += (s, _) => this.Close(); 
            sb.Begin();
        }

        private void SendFileButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = null;

            if (mw.ADBFirstDevice.IsChecked == true)
                selectedDevice = mw.ADBFirstDevice.Content.ToString();
            else if (mw.ADBSecondDevice.IsChecked == true)
                selectedDevice = mw.ADBSecondDevice.Content.ToString();
            else if (mw.ADBThirdDevice.IsChecked == true)
                selectedDevice = mw.ADBThirdDevice.Content.ToString();

            if (selectedDevice == null)
                MessageBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Title = "Select file to send",
                        Multiselect = false
                    };
                    dialog.ShowDialog();

                    string command = $"adb -s {selectedDevice} push {dialog.FileName} /storage/emulated/0/Download";

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",
                        WorkingDirectory = adbpath,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    };

                    Process.Start(psi);
                    if (selectedDevice == null)
                        MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                    else
                        MessageBox.Show("File Sended : " + selectedDevice);
                }
                else
                {
                    MessageBox.Show("adb.exe not found");
                }
            }
        }

        private void GetFileButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
