using ADBFastbootGUI.Themes;
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
    public partial class ProgramUninstallDialog : Window
    {
        MainWindow mw = new MainWindow();
        string adbpath = MainWindow.adbpath;

        RadioButton radioButton;
        System.Windows.Media.Brush foregroundBrush;

        private List<string> packageNames = new List<string>();

        public ProgramUninstallDialog()
        {
            InitializeComponent();
            string selectedDevice = mw.GetSelectedAdbDevice();
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
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                Process process = new Process
                {
                    StartInfo = psi,
                    EnableRaisingEvents = true
                };

                process.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data) && e.Data.StartsWith("package:"))
                    {
                        string packageName = e.Data.Substring(8);
                        packageNames.Add(packageName);
                    }
                };

                process.ErrorDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        packageNames.Add("Error: " + e.Data);
                    }
                };

                process.Exited += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        foreach (var name in packageNames)
                        {
                            radioButton = new RadioButton
                            {
                                Content = name,
                                GroupName = "PackageGroup",
                                Margin = new Thickness(5, 2, 5, 2),
                                Foreground = foregroundBrush,
                                HorizontalAlignment = HorizontalAlignment.Stretch
                            };

                            PackageListPanel.Children.Add(radioButton);
                        }
                    });
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            else
                DialogBox.Show("adb.exe Not found in path : " + adbpath);

            ThemeManagerHelper.ThemeChanged += OnThemeChanged;

            // 2. Hafıza sızıntılarını önlemek için pencere kapandığında dinlemeyi bırak.
            //    Bu çok önemlidir!
            this.Unloaded += (s, e) => ThemeManagerHelper.ThemeChanged -= OnThemeChanged;

            // 3. Pencere ilk açılırken mevcut temayı hemen uygula.
            //    Bunu yapmazsak, tema değişikliği olmadan pencere varsayılan renkte açılır.
            ChangeTheme(ThemeManagerHelper.IsDarkTheme);


            void OnThemeChanged(bool isDark)
            {
                // Gelen bilgiye göre renkleri değiştiren metodu çağır.
                ChangeTheme(isDark);
            }

            void ChangeTheme(bool isDark)
            {
                if (isDark)
                {
                    radioButton = new RadioButton();
                    // Koyu Tema
                    foregroundBrush = System.Windows.Media.Brushes.White;
                    this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                    SearchBox.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                }
                else
                {
                    // Açık Tema
                    foregroundBrush = System.Windows.Media.Brushes.Black;
                    this.Background = System.Windows.Media.Brushes.WhiteSmoke; // Örnek arkaplan
                }

                Title.Foreground = foregroundBrush;
                KeepDataCheckBox.Foreground = foregroundBrush;
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

        private void UninstallAppButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = null;

            if (mw.ADBFirstDevice.IsChecked == true)
                selectedDevice = mw.ADBFirstDevice.Content.ToString();
            else if (mw.ADBSecondDevice.IsChecked == true)
                selectedDevice = mw.ADBSecondDevice.Content.ToString();
            else if (mw.ADBThirdDevice.IsChecked == true)
                selectedDevice = mw.ADBThirdDevice.Content.ToString();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    string uninstallItem = PackageListPanel.Children.OfType<RadioButton>()
                        .FirstOrDefault(rb => rb.IsChecked == true)?.Content?.ToString() ?? "";

                    string output = uninstallItem;
                    string prefix = "packages:";

                    if (output.StartsWith(prefix))
                    {
                        output = output.Substring(prefix.Length).Trim();
                    }

                    string argument = null;
                    if (KeepDataCheckBox.IsChecked == true)
                    {
                        argument = $"/C adb -s {selectedDevice} shell pm uninstall -k --user 0 {output}";
                    }
                    else
                    {
                        argument = $"/C adb -s {selectedDevice} uninstall {output}";
                    }

                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = argument,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };

                    Process process = new Process
                    {
                        StartInfo = psi,
                        EnableRaisingEvents = true
                    };

                    process.OutputDataReceived += (s, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                var result = DialogBox.Show($"{args.Data}");

                                if (result == true)
                                {
                                    var sb = new Storyboard();

                                    var opacityAnim = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(200))
                                    {
                                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                                    };

                                    var topAnim = new DoubleAnimation(this.Top, this.Top - 20,
                                        TimeSpan.FromMilliseconds(200))
                                    {
                                        EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
                                    };

                                    sb.Children.Add(opacityAnim);
                                    sb.Children.Add(topAnim);

                                    Storyboard.SetTarget(opacityAnim, this);
                                    Storyboard.SetTargetProperty(opacityAnim, new PropertyPath("Opacity"));

                                    Storyboard.SetTarget(topAnim, this);
                                    Storyboard.SetTargetProperty(topAnim, new PropertyPath("Top"));

                                    sb.Completed += (a, _) =>
                                    {
                                        Application.Current.MainWindow.Opacity = 1;
                                        this.Close();
                                    };
                                    sb.Begin();
                                }
                            });
                        }
                    };

                    process.ErrorDataReceived += (s, args) =>
                    {
                        if (!string.IsNullOrEmpty(args.Data))
                        {
                            DialogBox.Show($"Uninstall Error: {args.Data}");
                        }
                    };

                    process.Start();
                    process.BeginErrorReadLine();
                    process.BeginOutputReadLine();
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in path: " + adbpath);
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            foreach (RadioButton rb in PackageListPanel.Children.OfType<RadioButton>())
            {
                if (rb.Content != null)
                {
                    string packageName = rb.Content.ToString().ToLower();
                    rb.Visibility = packageName.Contains(searchText) ? Visibility.Visible : Visibility.Collapsed;
                }
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
    }
}