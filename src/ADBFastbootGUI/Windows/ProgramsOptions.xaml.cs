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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ADBFastbootGUI.Windows;
using System.Windows.Media.Animation;

namespace ADBFastbootGUI.Windows
{
    /// <summary>
    /// Interaction logic for ProgramsOptions.xaml
    /// </summary>
    public partial class ProgramsOptions : Window
    {
        MainWindow mw = new MainWindow();
        public int button_number;
        string adbpath = $@"C:\Program Files\ADBFastbootGUI\";
        public ProgramsOptions()
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
            button_number = 3;
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

        private void UninstallProgramButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void InstallProgramButton_Click(object sender, RoutedEventArgs e)
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
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select Android Package Kit (APK)",
                    Filter = "Android Package Kit (.apk)|*.apk",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };
                path.ShowDialog();

                string command = $"adb -s {selectedDevice} install {path.FileName}";

                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k {command}",
                        WorkingDirectory = adbpath,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };

                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show("adb.exe Not found in path : " + adbpath);
                }
            }
        }

        private void ListProgramsButton_Click(object sender, RoutedEventArgs e)
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
                ProgramsListWindow plw = new ProgramsListWindow();
                plw.Owner = Application.Current.MainWindow;
                Opacity = 0.4;
                plw.ShowDialog();
            }
        }
    }
}
