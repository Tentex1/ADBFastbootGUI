using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ADBFastbootGUI.Windows.PartitionManagement
{
    /// <summary>
    /// CreateLogicalPartitionWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class CreateLogicalPartitionWindow : Window
    {
        MainWindow mw;
        public CreateLogicalPartitionWindow(Window wnd)
        {
            InitializeComponent();
            Owner = wnd;
        }

        private void CreatePartitionButton_Click(object sender, RoutedEventArgs e)
        {
            mw = new MainWindow();
            string selectedDevice = mw.GetSelectedFastbootDevice();

            string control = System.IO.Path.Combine(MainWindow.adbpath, "fastboot.exe");
            if (System.IO.File.Exists(control))
            {
                if (selectedDevice != null)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/C fastboot -s {selectedDevice} create-logical-partition {PartitionNameTextBox.Text} {PartitionSizeTextBox.Text}",
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    Process proc = new Process
                    {
                        StartInfo = psi,
                        EnableRaisingEvents = true
                    };

                    proc.OutputDataReceived += (s, odrargs) =>
                    {
                        DialogBox.Show($"{selectedDevice}: The partition '{PartitionNameTextBox.Text}' was created successfully.");
                    };

                    proc.ErrorDataReceived += (s, edrargs) =>
                    {
                        DialogBox.Show($"{selectedDevice}: The partition '{PartitionNameTextBox.Text}' could not be created due to error: " + edrargs.Data);
                    };

                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                }
                else
                    DialogBox.Show("CONNECT OR SELECT A DEVİCE!");
            }
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

            sb.Completed += (s, _) =>
            {
                Application.Current.MainWindow.Opacity = 1;
                this.Close();
            };
            sb.Begin();
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
    }
}
