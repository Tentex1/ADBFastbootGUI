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
    /// Interaction logic for ProgramsListWindow.xaml
    /// </summary>
    public partial class ProgramsListWindow : Window
    {
        MainWindow mw = new MainWindow();
        string adbpath = $@"C:\Program Files\ADBFastbootGUI\";
        public ProgramsListWindow()
        {
            InitializeComponent();

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
                    string command = $"adb -s {selectedDevice} shell pm list packages";
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C {command}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process process = new Process
                    {
                        StartInfo = psi,
                        EnableRaisingEvents = true
                    };

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                PackagesBox.AppendText($"\n\n{e.Data}" + "\n");
                                PackagesBox.ScrollToEnd();
                            });
                        }
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                PackagesBox.AppendText("\n\n" + e.Data + "\n");
                                PackagesBox.ScrollToEnd();
                            });
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }
                else
                    MessageBox.Show("adb.exe Not found in path : " + adbpath);
            }
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

        private void ExportPackageIDsButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog
            {
                FileName = "PackageID's",
                DefaultExt = ".txt",
                Filter = "Text files (.txt) |*.txt "
            };

            bool? result = file.ShowDialog();

            if (result == true)
            {
                File.WriteAllText(file.FileName, PackagesBox.Text);
                MessageBox.Show("Package ID's exported!");
            }
        }
    }
}
