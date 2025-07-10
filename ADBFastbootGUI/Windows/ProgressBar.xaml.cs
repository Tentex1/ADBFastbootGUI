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
using System.Windows.Threading;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Media.Animation;

namespace ADBFastbootGUI.Windows
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressBar : Window
    {
        private DispatcherTimer _progressTimer;
        private DispatcherTimer _dotTimer;
        private int _progressValue = 0;
        private int _dotStage = 0;
        private readonly string[] _dotStates = { "ADB Server Starting", "ADB Server Starting.", "ADB Server Starting..", "ADB Server Starting..."};
        public ProgressBar()
        {
            InitializeComponent();
            StartProgressBar();

            string adbpath = $@"C:\Program Files\ADBFastbootGUI\";
            string komut = "adb start-server"; 

            if (Directory.Exists(adbpath))
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/k {komut}",
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
                Info.FontSize = 15;
                Info.Text = "Reinstall this Program. Thanks for your Understanding";
                _progressTimer.Stop();
                _dotTimer.Stop();
                MessageBoxResult result = MessageBox.Show("Folder Not found: " + adbpath);
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
        private void StartProgressBar()
        {
            _progressTimer = new DispatcherTimer();
            _progressTimer.Interval = TimeSpan.FromMilliseconds(30);
            _progressTimer.Tick += ProgressTick;
            _progressTimer.Start();

            
            _dotTimer = new DispatcherTimer();
            _dotTimer.Interval = TimeSpan.FromMilliseconds(500);
            _dotTimer.Tick += DotTick;
            _dotTimer.Start();
        }
        private void ProgressTick(object sender, EventArgs e)
        {
            if (_progressValue >= 100)
            {
                _progressTimer.Stop();
                _dotTimer.Stop();
                Info.Text = "ADB Server Started!";
                Thread.Sleep(1000);

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

            _progressValue++;
            ADBServerStartingProgressBar.Value = _progressValue;
        }
        private void DotTick(object sender, EventArgs e)
        {
            Info.Text = _dotStates[_dotStage];
            _dotStage = (_dotStage + 1) % _dotStates.Length;
        }
    }
}
