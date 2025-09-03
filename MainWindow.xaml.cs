using ADBFastbootGUI.Themes;
using ADBFastbootGUI.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace ADBFastbootGUI
{
    public partial class MainWindow : Window
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        private IntPtr notificationHandle;

        string programFiles = "Program Files (x86)";

        private SettingsWindow ssw;

        public List<string> deviceIds = new List<string>();

        public static string adbpath = $@".\";

        Process cmdprocess;

        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            InitializeComponent();
            LoadDevices();
            LoadFastbootDevices();

            ThemeManagerHelper.ThemeChanged += OnThemeChanged;

            this.Unloaded += (s, e) => ThemeManagerHelper.ThemeChanged -= OnThemeChanged;

            ChangeTheme(ThemeManagerHelper.IsDarkTheme);
        }
        private void OnThemeChanged(bool isDark)
        {
            ChangeTheme(isDark);
        }
        public void ChangeTheme(bool isDark)
        {
            System.Windows.Media.Brush foregroundBrush;

            if (isDark)
            {
                foregroundBrush = System.Windows.Media.Brushes.White;
                this.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                QSTabGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                ADBTabGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                FastbootTabGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                ScrcpyTabGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
                DevicesTabGrid.Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(45, 45, 48));
            }
            else
            {
                foregroundBrush = System.Windows.Media.Brushes.Black;
                this.Background = System.Windows.Media.Brushes.WhiteSmoke;
                QSTabGrid.Background = System.Windows.Media.Brushes.WhiteSmoke;
                ADBTabGrid.Background = System.Windows.Media.Brushes.WhiteSmoke;
                FastbootTabGrid.Background = System.Windows.Media.Brushes.WhiteSmoke;
                ScrcpyTabGrid.Background = System.Windows.Media.Brushes.WhiteSmoke;
                DevicesTabGrid.Background = System.Windows.Media.Brushes.WhiteSmoke;
            }

            CloseButton.Foreground = foregroundBrush;
            MinButton.Foreground = foregroundBrush;
            SettingsButton.Foreground = foregroundBrush;
            AboutMenuItem.Foreground = foregroundBrush;
            DevicesTab.Foreground = foregroundBrush;
            ScrcpyTab.Foreground = foregroundBrush;
            FastbootTab.Foreground = foregroundBrush;
            ADBTab.Foreground = foregroundBrush;
            QSTab.Foreground = foregroundBrush;
            RebootingText.Foreground = foregroundBrush;
            ServerText.Foreground = foregroundBrush;
            FRebootingText.Foreground = foregroundBrush;
            LockingText.Foreground = foregroundBrush;
        }

        public List<string> GetConnectedDeviceIDs()
        {
            var devices = new List<string>();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");

            foreach (ManagementObject device in searcher.Get())
            {
                string deviceId = device["DeviceID"]?.ToString();
                if (deviceId != null)
                {
                    devices.Add(deviceId);
                }
            }
            return devices;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                LoadDevices();
                LoadFastbootDevices();
            }
        }
        private void TopPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (ssw == null)
            {
                ssw = new SettingsWindow();
                ssw.Owner = this;
            }

            ssw.Owner = this;
            Opacity = 0.4;
            ssw.ShowDialog();
            ssw.Window_Loaded('s', e);
            Opacity = 1;
            ssw.Activate();
        }
        private string GetSelectedAdbDevice()
        {
            if (ADBFirstDevice.IsChecked == true && ADBFirstDevice.Content.ToString() != "Device Not Found")
                return ADBFirstDevice.Content.ToString();
            if (ADBSecondDevice.IsChecked == true && ADBSecondDevice.Content.ToString() != "Device Not Found")
                return ADBSecondDevice.Content.ToString();
            if (ADBThirdDevice.IsChecked == true && ADBThirdDevice.Content.ToString() != "Device Not Found")
                return ADBThirdDevice.Content.ToString(); 
            if (ADBFourthDevice.IsChecked == true && ADBFourthDevice.Content.ToString() != "Device Not Found")
                return ADBFourthDevice.Content.ToString();
            if (ADBFifthDevice.IsChecked == true && ADBFifthDevice.Content.ToString() != "Device Not Found")
                return ADBFifthDevice.Content.ToString();
            return null;
        }
        private string GetSelectedFastbootDevice()
        {
            if (FastbootFirstDevice.IsChecked == true && FastbootFirstDevice.Content.ToString() != "Device Not Found")
                return FastbootFirstDevice.Content.ToString();
            if (FastbootSecondDevice.IsChecked == true && FastbootSecondDevice.Content.ToString() != "Device Not Found")
                return FastbootSecondDevice.Content.ToString();
            if (FastbootThirdDevice.IsChecked == true && FastbootThirdDevice.Content.ToString() != "Device Not Found")
                return FastbootThirdDevice.Content.ToString();
            if (FastbootFourthDevice.IsChecked == true && FastbootFourthDevice.Content.ToString() != "Device Not Found")
                return FastbootFourthDevice.Content.ToString();
            if (FastbootFifthDevice.IsChecked == true && FastbootFifthDevice.Content.ToString() != "Device Not Found")
                return FastbootFifthDevice.Content.ToString();
            return null;
        }

        public void LoadDevices()
        {
            string[] lines;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C adb devices",
                    WorkingDirectory = adbpath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }

                string[] deviceIds = lines.Length > 1
                    ? lines.Skip(1)
                        .Where(line => line.Contains("device"))
                        .Select(line => line.Split('\t')[0])
                        .ToArray()
                    : new string[0];

                ADBFirstDevice.Content = deviceIds.Length > 0 ? deviceIds[0] : "Device Not Found";
                ADBSecondDevice.Content = deviceIds.Length > 1 ? deviceIds[1] : "Device Not Found";
                ADBThirdDevice.Content = deviceIds.Length > 2 ? deviceIds[2] : "Device Not Found";
                ADBFourthDevice.Content = deviceIds.Length > 3 ? deviceIds[3] : "Device Not Found";
                ADBFifthDevice.Content = deviceIds.Length > 4 ? deviceIds[4] : "Device Not Found";
                
                if (deviceIds.Length == 1)
                {
                    ADBFirstDevice.IsEnabled = false;
                    ADBSecondDevice.IsEnabled = false;
                    ADBThirdDevice.IsEnabled = false;
                    ADBFourthDevice.IsEnabled = false;
                    ADBFifthDevice.IsEnabled = false;
                    ADBFirstDevice.IsChecked = true;
                }
                else if (deviceIds.Length == 2)
                {
                    ADBFirstDevice.IsEnabled = true;
                    ADBSecondDevice.IsEnabled = true;
                    ADBThirdDevice.IsEnabled = false;
                    ADBFourthDevice.IsEnabled = false;
                    ADBFifthDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 3)
                {
                    ADBFirstDevice.IsEnabled = true;
                    ADBSecondDevice.IsEnabled = true;
                    ADBThirdDevice.IsEnabled = true;
                    ADBFourthDevice.IsEnabled = false;
                    ADBFifthDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 4)
                {
                    ADBFirstDevice.IsEnabled = true;
                    ADBSecondDevice.IsEnabled = true;
                    ADBThirdDevice.IsEnabled = true;
                    ADBFourthDevice.IsEnabled = true;
                    ADBFifthDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 0)
                {
                    ADBFirstDevice.IsEnabled = false;
                    ADBSecondDevice.IsEnabled = false;
                    ADBThirdDevice.IsEnabled = false;
                    ADBFourthDevice.IsEnabled = false;
                    ADBFifthDevice.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                DialogBox.Show("Error: " + ex.Message);
            }
        }
        public void LoadFastbootDevices()
        {
            string[] lines;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C fastboot devices",
                    WorkingDirectory = adbpath,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = Process.Start(psi))
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    lines = output.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                }

                string[] deviceIds = lines.Length > 0
                    ? lines
                        .Where(line => line.Contains("fastboot"))
                        .Select(line => line.Split('\t')[0])
                        .ToArray()
                    : new string[0];

                FastbootFirstDevice.Content = deviceIds.Length > 0 ? deviceIds[0] : "Device Not Found";
                FastbootSecondDevice.Content = deviceIds.Length > 1 ? deviceIds[1] : "Device Not Found";
                FastbootThirdDevice.Content = deviceIds.Length > 2 ? deviceIds[2] : "Device Not Found";
                FastbootFourthDevice.Content = deviceIds.Length > 3 ? deviceIds[3] : "Device Not Found";
                FastbootFifthDevice.Content = deviceIds.Length > 4 ? deviceIds[4] : "Device Not Found";

                if (deviceIds.Length == 1)
                {
                    FastbootFirstDevice.IsEnabled = true;
                    FastbootSecondDevice.IsEnabled = false;
                    FastbootThirdDevice.IsEnabled = false;
                    FastbootFourthDevice.IsEnabled = false;
                    FastbootFifthDevice.IsEnabled = false;
                    FastbootFirstDevice.IsChecked = true;
                }
                else if (deviceIds.Length == 2)
                {
                    FastbootFirstDevice.IsEnabled = true;
                    FastbootSecondDevice.IsEnabled = true;
                    FastbootThirdDevice.IsEnabled = false;
                    FastbootFourthDevice.IsEnabled = false;
                    FastbootFifthDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 3)
                {
                    FastbootFirstDevice.IsEnabled = true;
                    FastbootSecondDevice.IsEnabled = true;
                    FastbootThirdDevice.IsEnabled = true;
                    FastbootFourthDevice.IsEnabled = false;
                    FastbootFifthDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 4)
                {
                    FastbootFirstDevice.IsEnabled = true;
                    FastbootSecondDevice.IsEnabled = true;
                    FastbootThirdDevice.IsEnabled = true;
                    FastbootFourthDevice.IsEnabled = true;
                    FastbootFifthDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 0)
                {
                    FastbootFirstDevice.IsEnabled = false;
                    FastbootSecondDevice.IsEnabled = false;
                    FastbootThirdDevice.IsEnabled = false;
                    FastbootFourthDevice.IsEnabled = false;
                    FastbootFifthDevice.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                DialogBox.Show("Error: " + ex.Message);
            }
        }

        private async Task<string> Commander(string command, string runner = "cmd.exe", bool hideWindow = true, bool useShellExecute = false, bool redirectOutput = true, bool redirectError = true)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = runner,
                Arguments = $"{command}",
                UseShellExecute = useShellExecute,
                CreateNoWindow = hideWindow,
                RedirectStandardOutput = redirectOutput && !useShellExecute,
                RedirectStandardError = redirectError && !useShellExecute
            };

            cmdprocess = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            cmdprocess.Start();

            string output = "";
            string error = "";

            if (redirectOutput && !useShellExecute)
                output = await cmdprocess.StandardOutput.ReadToEndAsync();

            if (redirectError && !useShellExecute)
                error = await cmdprocess.StandardError.ReadToEndAsync();

            await Task.Run(() => cmdprocess.WaitForExit());

            return output + error;
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            string selectedId = null;

            if (ADBFirstDevice.IsChecked == true && ADBFirstDevice.IsEnabled)
                selectedId = ADBFirstDevice.Content.ToString();
            else if (ADBSecondDevice.IsChecked == true && ADBSecondDevice.IsEnabled)
                selectedId = ADBSecondDevice.Content.ToString();
            else if (ADBThirdDevice.IsChecked == true && ADBThirdDevice.IsEnabled)
                selectedId = ADBThirdDevice.Content.ToString();

            if (string.IsNullOrEmpty(selectedId))
            {
                if (FastbootFirstDevice.IsChecked == true && FastbootFirstDevice.IsEnabled)
                    selectedId = FastbootFirstDevice.Content.ToString();
                else if (FastbootSecondDevice.IsChecked == true && FastbootSecondDevice.IsEnabled)
                    selectedId = FastbootSecondDevice.Content.ToString();
                else if (FastbootThirdDevice.IsChecked == true && FastbootThirdDevice.IsEnabled)
                    selectedId = FastbootThirdDevice.Content.ToString();
            }

            if (string.IsNullOrEmpty(selectedId) || selectedId == "Device Not Found")
            {
                DialogBox.Show("Please select a valid device.");
                return;
            }

            DialogBox.Show("Selected Device ID: " + selectedId);
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Windows.ProgressBar pbar = new Windows.ProgressBar
            {
                Owner = this
            };
            Opacity = 0.4;
            pbar.ShowDialog();
            Opacity = 1;

            var windowHandle = new WindowInteropHelper(this).Handle;
            var source = HwndSource.FromHwnd(windowHandle);
            source.AddHook(WndProc);

            RegisterUsbDeviceNotification(windowHandle);
        }

        public void RegisterUsbDeviceNotification(IntPtr windowHandle)
        {
            DEV_BROADCAST_DEVICEINTERFACE dbi = new DEV_BROADCAST_DEVICEINTERFACE
            {
                dbcc_size = Marshal.SizeOf(typeof(DEV_BROADCAST_DEVICEINTERFACE)),
                dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE,
                dbcc_classguid = GUID_DEVINTERFACE_USB_DEVICE
            };

            IntPtr buffer = Marshal.AllocHGlobal(dbi.dbcc_size);
            Marshal.StructureToPtr(dbi, buffer, true);

            notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
        }

        private static readonly Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_DEVICECHANGE)
            {
                int eventType = wParam.ToInt32();
                if (eventType == DBT_DEVICEARRIVAL)
                {
                    Thread.Sleep(1500);
                    LoadDevices();
                    LoadFastbootDevices();
                }
                else if (eventType == DBT_DEVICEREMOVECOMPLETE)
                {
                    LoadDevices();
                    LoadFastbootDevices();
                }
            }

            return IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            public short dbcc_name;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

        private async void ShellButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();
            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/K adb -s {selectedDevice} shell", "cmd.exe", false, true);
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program Path");
                }
            }
        }

        private async void KillServerButton_Click(object sender, RoutedEventArgs e)
        {
            string Control = System.IO.Path.Combine(adbpath, "adb.exe");
            if (File.Exists(Control))
            {
                string output = await Commander($"/C adb kill-server", "cmd.exe");
                if (output == "")
                {
                    DialogBox.Show("Server killed. (closed)");
                }
                else
                {
                    DialogBox.Show("Server already killed. (closed)");
                }
            }
            else
            {
                DialogBox.Show("adb.exe Not found in path : " + adbpath);
            }
        }

        private async void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            string Control = System.IO.Path.Combine(adbpath, "adb.exe");
            if (File.Exists(Control))
            {
                string output = await Commander($"/C adb start-server", "cmd.exe", true, true, false, false);
                DialogBox.Show(output);
                if (output.Contains("*"))
                {
                    DialogBox.Show("Server Started.");
                }
                else if (output == null)
                {
                    DialogBox.Show("Server already working.");
                }
                else
                {
                    DialogBox.Show("Error: " + output);
                }
            }
            else
            {
                DialogBox.Show("adb.exe Not found in Program path");
            }
        }

        private async void FastbootSidelodButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                        Title = "Select Image to sideload",
                        Filter = "Image (.img) | *.img",
                        Multiselect = false
                    };
                    dialog.ShowDialog();

                    if (File.Exists(Control))
                    {
                        string output = await Commander($"/C adb -s {selectedDevice} sideload {dialog.FileName}", "cmd.exe", true, true, false, false);

                        if (output.Contains("serving"))
                        {
                            DialogBox.Show($"{selectedDevice}: Sideloading {System.IO.Path.GetFileName(dialog.FileName)}");
                        }
                        else
                        {
                            DialogBox.Show("Sideload failed: " + output);
                        }
                    }
                    else
                    {
                        DialogBox.Show("adb.exe Not found in Program path");
                    }
                }
            }
        }

        private void Header_Click(object sender, RoutedEventArgs e)
        {
            AboutBox ab = new AboutBox();
            Opacity = 0.4;
            ab.ShowDialog();
            Opacity = 1;
        }

        private async void ARecoveryRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C adb -s {selectedDevice} reboot recovery", "cmd.exe", true, false);

                    if (string.IsNullOrWhiteSpace(output))
                        DialogBox.Show($"{selectedDevice}: Rebooting Recovery.");
                    else
                        DialogBox.Show("Error: " + output);
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path");
                }
            }
        }

        private async void ASystemRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/K adb -s {selectedDevice} reboot", "cmd.exe", true, false);

                    if (string.IsNullOrWhiteSpace(output))
                        DialogBox.Show($"{selectedDevice}: Rebooting.");
                    else
                        DialogBox.Show("Error: " + output);
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Proram path");
                }
            }
        }

        private async void ABootloaderRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C adb -s {selectedDevice} reboot bootloader", "cmd.exe", true, false);

                    if (string.IsNullOrWhiteSpace(output))
                        DialogBox.Show($"{selectedDevice}: Rebooting Bootloader.");
                    else
                        DialogBox.Show("Error: " + output);
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path.");
                }
            }
        }
        private async void FImageRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    OpenFileDialog image = new OpenFileDialog
                    {
                        Title = "Select Image to boot",
                        Filter = "Image (.img) | *.img",
                        Multiselect = false,
                        InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                    };
                    image.ShowDialog();

                    if (image.ShowDialog() == true)
                    {
                        string fastbootoutput = await Commander($"/C fastboot -s {selectedDevice} boot {image.FileName}", "cmd.exe", true);

                        if (fastbootoutput.Contains("Finished"))
                        {
                            DialogBox.Show($"{selectedDevice}: Booting image: {image.FileName}.");
                        }
                        else
                        {
                            DialogBox.Show("Boot failed: " + fastbootoutput);
                        }
                    }
                    else
                        DialogBox.Show("No file selected.");
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path");
                }
            }
        }

        private async void FBootloaderRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string fastbootoutput = await Commander($"/C fastboot -s {selectedDevice} reboot bootloader", "cmd.exe", true);

                    if (fastbootoutput.Contains("Finished"))
                    {
                        DialogBox.Show($"{selectedDevice}: Rebooting bootloader.");
                    }
                    else
                    {
                        DialogBox.Show("Reboot failed: " + fastbootoutput);
                    }
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path.");
                }
            }
        }

        private async void FSystemRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string fastbootoutput = await Commander($"/C fastboot -s {selectedDevice} reboot", "cmd.exe", true);

                    if (fastbootoutput.Contains("Finished"))
                    {
                        DialogBox.Show($"{selectedDevice}: Rebooting system.");
                    }
                    else
                    {
                        DialogBox.Show("Reboot failed: " + fastbootoutput);
                    }
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path.");
                }
            }
        }

        private async void FRecoveryRebootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (FastbootFirstDevice.IsChecked == true)
                selectedDevice = FastbootFirstDevice.Content.ToString();
            else if (FastbootSecondDevice.IsChecked == true)
                selectedDevice = FastbootSecondDevice.Content.ToString();
            else if (FastbootThirdDevice.IsChecked == true)
                selectedDevice = FastbootThirdDevice.Content.ToString();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string fastbootoutput = await Commander($"/C fastboot -s {selectedDevice} reboot recovery", "cmd.exe", true, false);

                    if (fastbootoutput.Contains("Finished"))
                    {
                        DialogBox.Show($"{selectedDevice}: Rebooting recovery.");
                    }
                    else
                    {
                        DialogBox.Show("Reboot failed: " + fastbootoutput);
                    }
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path.");
                }
            }
        }

        private async void InstallAppButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select Android Package Kit (APK)",
                    Filter = "Android Package Kit (.apk)|*.apk",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };

                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    if (path.ShowDialog() == true)
                    {
                        string output = await Commander($"/C adb -s {selectedDevice} install {path.FileName}", "cmd.exe", true);

                        if (output.Contains("Success"))
                            DialogBox.Show($"{selectedDevice}: {Path.GetFileName(path.FileName)} Installed.");
                        else
                            DialogBox.Show("Error: " + output);
                    }
                    else
                        DialogBox.Show("No file selected.");
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path");
                }
            }
        }

        private void UninstallAppButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramUninstallDialog pud = new ProgramUninstallDialog();
            pud.Owner = Application.Current.MainWindow;
            Opacity = 0.4;
            pud.ShowDialog();
            Opacity = 1;
        }

        private void ListAppsButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                ProgramsListWindow plw = new ProgramsListWindow();
                plw.Owner = Application.Current.MainWindow;
                Opacity = 0.4;
                plw.ShowDialog();
            }
        }

        private async void SendFileButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
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

                    if (dialog.ShowDialog() == true)
                    {
                        string output = await Commander($"/C adb -s {selectedDevice} push {dialog.FileName} /storage/emulated/0/Download");

                        if (output.Contains($"{Path.GetFileName(dialog.FileName)}"))
                            DialogBox.Show($"{selectedDevice}: {Path.GetFileName(dialog.FileName)} pushed succesfully.");
                        else
                            DialogBox.Show("Error: " + output);
                    }
                    else
                        DialogBox.Show("No file selected.");
                }
                else
                {
                    DialogBox.Show("adb.exe not found in path Program Path");
                }
            }
        }
        private async void FlashRecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
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

                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C fastboot -s {selectedDevice} flash recovery {path.FileName}");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");

                    if (path.FileName != null)
                    {
                        DialogBox.Show($"{selectedDevice}: {System.IO.Path.GetFileName(path.FileName)} flashed in 'recovery' partition.");
                    }
                    else
                    {
                        DialogBox.Show("No file selected.");
                    }
                    
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void FlashBootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select Boot Image (boot.img)",
                    Filter = "Image (.img)|*.img",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };
                path.ShowDialog();

                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C fastboot -s {selectedDevice} flash boot {path.FileName}");
                    
                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                    
                    if (path.FileName != null)
                    {
                        DialogBox.Show($"{selectedDevice}: {System.IO.Path.GetFileName(path.FileName)} flashed in 'boot' partition.");
                    }
                    else
                    {
                        DialogBox.Show("No file selected.");
                    }
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void FlashSystemButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select System Image (system.img)",
                    Filter = "Image (.img)|*.img",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };
                path.ShowDialog();

                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C fastboot -s {selectedDevice} flash system {path.FileName}");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");

                    if (path.FileName != null)
                    {
                        DialogBox.Show($"{selectedDevice}: {System.IO.Path.GetFileName(path.FileName)} flashed in 'system' partition.");
                    }
                    else
                    {
                        DialogBox.Show("No file selected.");
                    }
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void FlashVendorButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select Vendor Image (vendor.img)",
                    Filter = "Image (.img)|*.img",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };
                path.ShowDialog();

                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C fastboot -s {selectedDevice} flash vendor {path.FileName}");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");

                    if (path.FileName != null)
                    {
                        DialogBox.Show($"{selectedDevice}: {System.IO.Path.GetFileName(path.FileName)} flashed in 'vendor' partition.");
                    }
                    else
                    {
                        DialogBox.Show("No file selected.");
                    }

                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void FlashVbmetaButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select Vbmeta Image (vbmeta.img)",
                    Filter = "Image (.img)|*.img",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };
                path.ShowDialog();

                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C fastboot -s {selectedDevice} flash vbmeta {path.FileName}");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");

                    if (path.FileName != null)
                    {
                        DialogBox.Show($"{selectedDevice}: {System.IO.Path.GetFileName(path.FileName)} flashed in 'vbmeta' partition.");
                    }
                    else
                    {
                        DialogBox.Show("No file selected.");
                    }

                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }
        private async void FlashDTBOButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                OpenFileDialog path = new OpenFileDialog
                {
                    Title = "Select Vbmeta Image (vbmeta.img)",
                    Filter = "Image (.img)|*.img",
                    Multiselect = false,
                    InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                };
                path.ShowDialog();

                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    string output = await Commander($"/C fastboot -s {selectedDevice} flash dtbo {path.FileName}");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");

                    if (path.FileName != null)
                    {
                        DialogBox.Show($"{selectedDevice}: {System.IO.Path.GetFileName(path.FileName)} flashed in 'dtbo' partition.");
                    }
                    else
                    {
                        DialogBox.Show("No file selected.");
                    }

                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void OEMUnlockButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {

                    var result = DialogBox.Show($"All data on your device will be deleted. Do you continue?", DialogBoxButton.YesCancel);
                    if (result == true)
                    {
                        string output = await Commander($"/C fastboot -s {selectedDevice} oem unlock");
                        string output2 = await Commander($"/C fastboot -s {selectedDevice} flashing unlock");
                    }
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path");
                }
            }
        }


        private async void OEMLockButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    var result = DialogBox.Show("All data on your device will be deleted. Do you continue?", DialogBoxButton.YesCancel);

                    if (result == true)
                    {
                        string output = await Commander("/K fastboot flashing lock", "cmd.exe", false, true);
                        string output2 = await Commander("/K fastboot oem lock", "cmd.exe", false, true);
                    }
                }
                else
                {
                    DialogBox.Show("adb.exe Not found in Program path");
                }
            }
        }
        private async void StartRecordButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            string selectedFormat = null;

            if (OutputFormatComboBox.SelectedIndex == 0)
                selectedFormat = "mp4";
            else if (OutputFormatComboBox.SelectedIndex == 1)
                selectedFormat = "mkv";

            string videoCodec = null;

            if (ScrcpyConfigVideoCodecComboBox.SelectedIndex == 0)
                videoCodec = "h264";
            else if (ScrcpyConfigVideoCodecComboBox.SelectedIndex == 1)
                videoCodec = "h265";
            else if (ScrcpyConfigVideoCodecComboBox.SelectedIndex == 2)
                videoCodec = "av1";

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                if (RecordVideoCheckBox.IsChecked == true && RecordAudioCheckBox.IsChecked == true)
                {
                    string Control = System.IO.Path.Combine(adbpath, "scrcpy.exe");
                    if (File.Exists(Control))
                    {
                        if (OutputFormatComboBox.SelectedItem != null)
                        {
                            var result = DialogBox.Show($"{selectedDevice}: Starting device record and mirroring screen.", DialogBoxButton.YesCancel);
                            if (result == true)
                            {
                                string output = await Commander($@"/C scrcpy.exe -s {selectedDevice} --max-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --video-codec={videoCodec} --audio-codec=aac --record=C:\Users\{Environment.UserName}\Desktop\file_{selectedDevice}.{selectedFormat}", "cmd.exe", true, false);
                                DialogBox.Show("Recording saved to Desktop.");
                            }
                        }
                        else
                        {
                            DialogBox.Show("No output format selected.");
                        }

                    }
                    else
                    {
                        DialogBox.Show("scrcpy.exe Not found in Program Path");
                    }
                }
                else if (RecordVideoCheckBox.IsChecked == false && RecordAudioCheckBox.IsChecked == true)
                {
                    string Control = System.IO.Path.Combine(adbpath, "scrcpy.exe");
                    if (File.Exists(Control))
                    {
                        DialogBox.Show($"{selectedDevice} Starting audio record and mirroring screen.");
                        string output = await Commander($@"/C scrcpy -s {selectedDevice} --no-video --audio-codec=raw --record=C:\Users\{Environment.UserName}\Desktop\record_{selectedDevice}.wav", "cmd.exe", true, false, true, true);
                        DialogBox.Show("Recording saved to Desktop.");
                    }
                    else
                    {
                        DialogBox.Show("scrcpy.exe Not found in path : " + adbpath);
                    }
                }
                else if (RecordVideoCheckBox.IsChecked == true && RecordAudioCheckBox.IsChecked == false)
                {
                    string Control = System.IO.Path.Combine(adbpath, "scrcpy.exe");
                    if (File.Exists(Control))
                    {
                        string output = await Commander($@"/C scrcpy -s {selectedDevice} --no-audio --record=C:\Users\{Environment.UserName}\Desktop\record.mp4");
                        DialogBox.Show("Recording saved to Desktop.");
                    }
                    else
                    {
                        DialogBox.Show("scrcpy.exe Not found in path : " + adbpath);
                    }
                }
                else
                {
                    DialogBox.Show("YOU HAVE MADE NO CHOICE!");
                }
            }
        }

        private async void StartScreenMirroringButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "scrcpy.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Screen Mirror starting.");
                    string output = await Commander("/C scrcpy.exe");
                }
                else
                {
                    DialogBox.Show("scrcpy.exe Not found in path : " + adbpath);
                }
            }
        }
        private async void StartOTGButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "scrcpy.exe");
                if (File.Exists(Control))
                {
                    int selection = 0;

                    var selindex = ScrcpyOTGOption.SelectedIndex;

                    if (selindex == 0)
                    {
                        selection = 1;
                    }
                    else if (selindex == 1)
                    {
                        selection = 2;
                    }
                    else
                    {
                        DialogBox.Show("Please select a OTG mode option.");
                        return;
                    }

                    string commandSwitch;

                    switch (selection)
                    {
                        case 1:
                            var resultMK = DialogBox.Show($"{selectedDevice}: Start OTG mode (Mouse, Keyboard)?", DialogBoxButton.YesCancel);

                            if (resultMK == true)
                            {
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --otg";
                            }
                            else
                            {
                                return;
                            }

                            break;

                        case 2:
                            var resultG = DialogBox.Show($"{selectedDevice}: Start OTG mode (Gamepad)?", DialogBoxButton.YesCancel);
                            if (resultG == true)
                            {
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --gamepad=uhid";
                            }
                            else
                            {
                                return;
                            }
                            break;

                        default:
                            DialogBox.Show("Please select an option for OTG mode.");
                            return;
                    }

                    string output = await Commander("/C " + commandSwitch);
                }
                else
                {
                    DialogBox.Show("scrcpy.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseDTBOButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'dtbo' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase dtbo");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseVbmetaButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'vbmeta' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase vbmeta");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseVendorButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'vendor' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase vendor");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseSystemButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'system' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase system");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseBootButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'boot' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase boot");
                    
                    if (IsRebootEnded.IsChecked== true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");

                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseRecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'recovery' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase recovery");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void EraseUserdataButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'recovery' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} erase userdata");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void FlashUserdataButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedFastbootDevice();

            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select Userdata image (userdata.img)",
                Filter = "Image(.img) | *.img",
                InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
            };
            dialog.ShowDialog();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                if (File.Exists(Control))
                {
                    DialogBox.Show($"{selectedDevice}: Erasing 'recovery' partition.");
                    string output = await Commander($"/C fastboot.exe -s {selectedDevice} flash userdata {dialog.FileName}");

                    if (IsRebootEnded.IsChecked == true)
                        await Commander($"/C fastboot.exe -s {selectedDevice} reboot");
                }
                else
                {
                    DialogBox.Show("fastboot.exe Not found in path : " + adbpath);
                }
            }
        }

        private async void StartMirroringButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            string videoCodec = null;

            if (ScrcpyConfigVideoCodecComboBox.SelectedIndex == 0)
                videoCodec = "h264";
            else if (ScrcpyConfigVideoCodecComboBox.SelectedIndex == 1)
                videoCodec = "h265";
            else if (ScrcpyConfigVideoCodecComboBox.SelectedIndex == 2)
                videoCodec = "av1";

            int selection = 0;

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string path = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(path))
                {
                    if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 0 && ScrcpyMirroringMirrorAudioCheckBox.IsChecked == true)
                    {
                        selection = 1; // Mirror screen with audio
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 1 && ScrcpyMirroringMirrorAudioCheckBox.IsChecked == true)
                    {
                        selection = 2; // Mirror front cam with audio
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 2 && ScrcpyMirroringMirrorAudioCheckBox.IsChecked == true)
                    {
                        selection = 3; // Mirror back cam with audio
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 3 && ScrcpyMirroringMirrorAudioCheckBox.IsChecked == true)
                    {
                        selection = 4; // Mirror external cam with audio
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 0)
                    {
                        selection = 5; // Mirror screen
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 1)
                    {
                        selection = 6; // Mirror front cam
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 2)
                    {
                        selection = 7; // Mirror back cam
                    }
                    else if (ScrcpyMirroringMirrorItemComboBox.SelectedIndex == 3)
                    {
                        selection = 8; // Mirror external cam
                    }
                    else
                    {
                        DialogBox.Show("Please select a valid option for mirroring.");
                        return;
                    }

                    string commandSwitch = null;

                    switch (selection)
                    {
                        case 1:
                            var resultASM = DialogBox.Show($"{selectedDevice}: Start screen mirroring with audio?", DialogBoxButton.YesCancel);
                            if (resultASM == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --max-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --video-codec={videoCodec}";
                            break;

                        case 2:
                            var resultACF = DialogBox.Show($"{selectedDevice}: Start front camera mirroring with audio?", DialogBoxButton.YesCancel);
                            if (resultACF == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --video-source=camera --camera-facing=front --camera-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --audio-source=output";
                            break;

                        case 3:
                            var resultACB = DialogBox.Show($"{selectedDevice}: Start back camera mirroring with audio?", DialogBoxButton.YesCancel);
                            if (resultACB == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --video-source=camera --camera-facing=back --camera-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --audio-source=output";
                            break;

                        case 4:
                            var resultACE = DialogBox.Show($"{selectedDevice}: Start external camera mirroring with audio?", DialogBoxButton.YesCancel);
                            if (resultACE == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --video-source=camera --camera-facing=external --camera-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --audio-source=output";
                            break;

                        case 5:
                            var resultSM = DialogBox.Show($"{selectedDevice}: Start screen mirroring without audio?", DialogBoxButton.YesCancel);
                            if (resultSM == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --max-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --video-codec={videoCodec} --no-audio";
                            break;

                        case 6:
                            var resultCF = DialogBox.Show($"{selectedDevice}: Start front camera mirroring without audio?", DialogBoxButton.YesCancel);
                            if (resultCF == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --video-source=camera --camera-facing=front --camera-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --no-audio";
                            break;

                        case 7:
                            var resultCB = DialogBox.Show($"{selectedDevice}: Start back camera mirroring without audio?", DialogBoxButton.YesCancel);
                            if (resultCB == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --video-source=camera --camera-facing=back --camera-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --no-audio";
                            break;

                        case 8:
                            var resultCE = DialogBox.Show($"{selectedDevice}: Start external camera mirroring without audio?", DialogBoxButton.YesCancel);
                            if (resultCE == true)
                                commandSwitch = $"scrcpy.exe -s {selectedDevice} --video-source=camera --camera-facing=external --camera-fps={Convert.ToInt16(ScrcpyConfigFPSTextBox.Text)} --no-audio";
                            break;

                        default:
                            DialogBox.Show("Invalid selection for mirroring.");
                            break;
                    }

                    string output = await Commander("/C " + commandSwitch, "cmd.exe");

                    if (output.Contains("Android 12") && selection == 2)
                        DialogBox.Show("The Camera Mirroring feature works on Android 12 and above.");
                    else if (output.Contains("Android 12") && selection == 3)
                        DialogBox.Show("The Camera Mirroring feature works on Android 12 and above.");
                    else if (output.Contains("Android 12") && selection == 4)
                        DialogBox.Show("The Camera Mirroring feature works on Android 12 and above.");
                    else if (output.Contains("Android 12") && selection == 6)
                        DialogBox.Show("The Camera Mirroring feature works on Android 12 and above.");
                    else if (output.Contains("Android 12") && selection == 7)
                        DialogBox.Show("The Camera Mirroring feature works on Android 12 and above.");
                    else if (output.Contains("Android 12") && selection == 8)
                        DialogBox.Show("The Camera Mirroring feature works on Android 12 and above.");

                }
                else
                {
                    Opacity = 0.4;
                    DialogBox.Show("scrcpy.exe Not found in path : " + adbpath);
                    Opacity = 1;
                }
            }
        }

        private void WirelessConnectButton_Click(object sender, RoutedEventArgs e)
        {
            WirelessConnectionWindow wcw = new WirelessConnectionWindow();
            Opacity = 0.4;
            wcw.Owner = this;
            wcw.ShowDialog();
            Opacity = 1;
        }

        private async void LoggingButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVİCE!");
            else
            {
                string control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(control))
                {
                    var result = DialogBox.Show("Would you like to see the logs?", DialogBoxButton.YesCancel);
                    if (result == true)
                    {
                        string output = await Commander($"-Command \"& .\\adb -s {selectedDevice} logcat | Tee-Object -FilePath 'C:\\Users\\{Environment.UserName}\\Desktop\\logcat_{selectedDevice}.txt'\"", "powershell.exe", false, true);

                        var pshell = Process.GetProcessesByName("powershell");
                        if (pshell.Length < 0) { }
                        else
                        {
                            DialogBox.Show("Log saved in: '" + Environment.UserName + "\\Desktop\\logcat_" + selectedDevice + ".txt'");
                        }
                    }
                }
            }
        }

        private void BackupButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();
        }

        private async void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVİCE!");
            else
            {
                string control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(control))
                {
                    OpenFileDialog dialog = new OpenFileDialog
                    {
                    };

                    string output = await Commander($"/C adb -s {selectedDevice} restore");

                    if (output.Contains($"{selectedDevice}"))
                    {
                        DialogBox.Show("Serial NO: " + output);
                    }
                    else
                    {
                        DialogBox.Show("Error: " + output);
                    }
                }
            }

        }

        private async void GetDeviceInfoButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = GetSelectedAdbDevice();

            if (selectedDevice == null)
                DialogBox.Show("CONNECT OR SELECT A DEVİCE!");
            else
            {
                string control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(control))
                {
                    string serialNoOutput = await Commander($"/C adb -s {selectedDevice} get-serialno");
                    string deviceModelOutput = await Commander($"/C adb -s {selectedDevice} shell getprop ro.product.model");
                    string manufacturerOutput = await Commander($"/C adb -s {selectedDevice} shell getprop ro.product.manufacturer ");

                    if (serialNoOutput.Contains($"{selectedDevice}"))
                    {
                        DialogBox.Show($"\n\nSerial NO: {serialNoOutput}\nDevice Model: {deviceModelOutput}\nManufacturer: {manufacturerOutput}\n", DialogBoxButton.OK, DialogBoxSize.WidthAndHeight);
                    }
                    else
                    {
                        DialogBox.Show("Error: " + serialNoOutput);
                    }
                }
            }
        }

        private void ScrcpyConfigFPSTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
