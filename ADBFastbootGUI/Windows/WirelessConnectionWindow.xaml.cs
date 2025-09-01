using ADBFastbootGUI.Themes;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ADBFastbootGUI.Windows
{
    /// <summary>
    /// WirelessConnectionWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class WirelessConnectionWindow : Window
    {
        private SettingsWindow sw = new SettingsWindow();
        private MainWindow mw = new MainWindow();
        public WirelessConnectionWindow()
        {
            InitializeComponent();

            ThemeManagerHelper.ThemeChanged += OnThemeChanged;

            // 2. Hafıza sızıntılarını önlemek için pencere kapandığında dinlemeyi bırak.
            //    Bu çok önemlidir!
            this.Unloaded += (s, e) => ThemeManagerHelper.ThemeChanged -= OnThemeChanged;

            // 3. Pencere ilk açılırken mevcut temayı hemen uygula.
            //    Bunu yapmazsak, tema değişikliği olmadan pencere varsayılan renkte açılır.
            ChangeTheme(ThemeManagerHelper.IsDarkTheme);
        }
        private void OnThemeChanged(bool isDark)
        {
            // Gelen bilgiye göre renkleri değiştiren metodu çağır.
            ChangeTheme(isDark);
        }
        public void ChangeTheme(bool isDark)
        {
            System.Windows.Media.Brush foregroundBrush; // Tek bir değişkenle kodu kısaltalım.

            if (isDark)
            {
                // Koyu Tema
                foregroundBrush = System.Windows.Media.Brushes.White;
                this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
            }
            else
            {
                // Açık Tema
                foregroundBrush = System.Windows.Media.Brushes.Black;
                this.Background = System.Windows.Media.Brushes.WhiteSmoke; // Örnek arkaplan
            }

            Header.Foreground = foregroundBrush;
            PlaceholderDeviceIP.Foreground = foregroundBrush;
            PlaceholderPairIP.Foreground = foregroundBrush;
            Info.Foreground = foregroundBrush;
            IsFirstConnection.Foreground = foregroundBrush;
        }
        private void TextBoxes_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsFirstConnection.IsChecked == false)
            {
                DeviceIPTextBox.IsEnabled = true;
                ConnectButton.IsEnabled = !string.IsNullOrWhiteSpace(PairIPTextBox.Text) && !string.IsNullOrWhiteSpace(DeviceIPTextBox.Text);
            }
            else
            {
                DeviceIPTextBox.IsEnabled = false;
                ConnectButton.IsEnabled = !string.IsNullOrWhiteSpace(PairIPTextBox.Text);
            }

        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string control = System.IO.Path.Combine(MainWindow.adbpath, "adb.exe");
            if (File.Exists(control))
            {
                ProcessStartInfo psiPair = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/C adb pair {PairIPTextBox.Text}",
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process processPair = new Process
                {
                    StartInfo = psiPair,
                    EnableRaisingEvents = true
                };

                processPair.OutputDataReceived += (s, args) =>
                {
                    if (args.Data.Contains("paired"))
                    {
                        ProcessStartInfo psiConnect = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/C adb devices",
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };

                        Process processConnect = new Process
                        {
                            StartInfo = psiConnect,
                            EnableRaisingEvents = true
                        };

                        processConnect.OutputDataReceived += (sndr, argmnt) =>
                        {
                            if (argmnt.Data.Contains("adb-"))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    mw.LoadDevices();
                                    mw.LoadFastbootDevices();

                                });
                            }
                        };

                        processConnect.ErrorDataReceived += (sdr, argmt) =>
                        {
                            if (!string.IsNullOrEmpty(argmt.Data))
                            {
                                Dispatcher.Invoke(() =>
                                {
                                    mw.LoadDevices();
                                    mw.LoadFastbootDevices();
                                });
                            }
                        };

                        processConnect.Start();
                        processConnect.BeginOutputReadLine();
                        processConnect.BeginErrorReadLine();
                    }

                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            mw.LoadDevices();
                            mw.LoadFastbootDevices();
                            DialogBox.Show($"Connected device.");

                        });
                    }
                };

                processPair.ErrorDataReceived += (s, args) =>
                {
                    if (!string.IsNullOrEmpty(args.Data))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            mw.LoadDevices();
                            mw.LoadFastbootDevices();
                            DialogBox.Show($"Error: {args.Data}");
                        });
                    }
                };

                processPair.Start();
                processPair.BeginOutputReadLine();
                processPair.BeginErrorReadLine();
            }
            else
            {
                DialogBox.Show("adb.exe is not found in Program Path.");
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

        private void IsFirstConnection_Checked(object sender, RoutedEventArgs e)
        {
            DeviceIPTextBox.IsEnabled = false;
            DeviceIPTextBox.Clear();
            ConnectButton.IsEnabled = !string.IsNullOrWhiteSpace(PairIPTextBox.Text);
        }

        private void IsFirstConnection_Unchecked(object sender, RoutedEventArgs e)
        {
            DeviceIPTextBox.IsEnabled = true;
            ConnectButton.IsEnabled = !string.IsNullOrWhiteSpace(PairIPTextBox.Text) && !string.IsNullOrWhiteSpace(DeviceIPTextBox.Text);
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("https://adbfastbootgui.vercel.app");
            Process.Start(uri.ToString());
        }
    }
}
