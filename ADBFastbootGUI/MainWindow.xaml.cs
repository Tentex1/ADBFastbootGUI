using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ADBFastbootGUI.Windows;
using System.Windows.Interop;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace ADBFastbootGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;

        private IntPtr notificationHandle;

        private List<string> deviceIds = new List<string>();
        string adbpath = $@"C:\Program Files\ADBFastbootGUI\";
        public MainWindow()
        {
            Loaded += MainWindow_Loaded;
            InitializeComponent();
            FindAndWriteDevices();
            LoadDevices();
            LoadFastbootDevices();
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                ADBDevicesTextBox.Clear();
                FastbootDevicesTextBox.Clear();
                FindAndWriteDevices();
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
        private void LoadDevices()
        {
            string[] lines;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "adb.exe",
                    Arguments = "devices",
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
                    :new string[0];

                ADBFirstDevice.Content = deviceIds.Length > 0 ? deviceIds[0] : "Device Not Found";
                ADBSecondDevice.Content = deviceIds.Length > 1 ? deviceIds[1] : "Device Not Found";
                ADBThirdDevice.Content = deviceIds.Length > 2 ? deviceIds[2] : "Device Not Found";

                if (deviceIds.Length == 1)
                {
                    ADBFirstDevice.IsEnabled = false;
                    ADBSecondDevice.IsEnabled = false;
                    ADBThirdDevice.IsEnabled = false;
                    ADBFirstDevice.IsChecked = true;
                }
                else if (deviceIds.Length == 2)
                {
                    ADBFirstDevice.IsEnabled = true;
                    ADBSecondDevice.IsEnabled = true;
                    ADBThirdDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 3)
                {
                    ADBFirstDevice.IsEnabled = true;
                    ADBSecondDevice.IsEnabled = true;
                    ADBThirdDevice.IsEnabled = true;
                }
                else if (deviceIds.Length == 0)
                {
                    ADBFirstDevice.IsEnabled = false;
                    ADBSecondDevice.IsEnabled = false;
                    ADBThirdDevice.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void LoadFastbootDevices()
        {
            string[] lines;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "fastboot.exe",
                    Arguments = "devices",
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

                if (deviceIds.Length == 1)
                {
                    FastbootFirstDevice.IsEnabled = false;
                    FastbootSecondDevice.IsEnabled = false;
                    FastbootThirdDevice.IsEnabled = false;
                    FastbootFirstDevice.IsChecked = true;
                }
                else if (deviceIds.Length == 2)
                {
                    FastbootFirstDevice.IsEnabled = true;
                    FastbootSecondDevice.IsEnabled = true;
                    FastbootThirdDevice.IsEnabled = false;
                }
                else if (deviceIds.Length == 3)
                {
                    FastbootFirstDevice.IsEnabled = true;
                    FastbootSecondDevice.IsEnabled = true;
                    FastbootThirdDevice.IsEnabled = true;
                }
                else if (deviceIds.Length == 0)
                {
                    FastbootFirstDevice.IsEnabled = false;
                    FastbootSecondDevice.IsEnabled = false;
                    FastbootThirdDevice.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private string Commander(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                    return output;
                else
                    return $"Error: {error}";
            }
        }

        private void OnApplyClicked(object sender, RoutedEventArgs e)
        {
            string selectedId = null;

            // Önce ADB cihazlarından biri seçili mi kontrol et
            if (ADBFirstDevice.IsChecked == true && ADBFirstDevice.IsEnabled)
                selectedId = ADBFirstDevice.Content.ToString();
            else if (ADBSecondDevice.IsChecked == true && ADBSecondDevice.IsEnabled)
                selectedId = ADBSecondDevice.Content.ToString();
            else if (ADBThirdDevice.IsChecked == true && ADBThirdDevice.IsEnabled)
                selectedId = ADBThirdDevice.Content.ToString();

            // Eğer ADB'den seçilmemişse fastboot'a bak
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
                MessageBox.Show("Please select a valid device.");
                return;
            }

            // Burada selectedId kullanılabilir
            MessageBox.Show("Selected Device ID: " + selectedId);
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

        private void RegisterUsbDeviceNotification(IntPtr windowHandle)
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
                    ADBDevicesTextBox.Clear();
                    FastbootDevicesTextBox.Clear();
                    FindAndWriteDevices();
                    LoadDevices();
                    LoadFastbootDevices();
                }
                else if (eventType == DBT_DEVICEREMOVECOMPLETE)
                {
                    ADBDevicesTextBox.Clear();
                    FastbootDevicesTextBox.Clear();
                    FindAndWriteDevices();
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

        private void FindAndWriteDevices()
        {
            string Control = System.IO.Path.Combine(adbpath, "adb.exe");
            if (File.Exists(Control))
            {
                ProcessStartInfo adbPSI = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C adb devices",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process adbProcess = new Process
                {
                    StartInfo = adbPSI,
                    EnableRaisingEvents = true
                };

                adbProcess.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ADBDevicesTextBox.AppendText(e.Data+ "\n");
                            ADBDevicesTextBox.ScrollToEnd();
                        });
                    }
                };

                adbProcess.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            ADBDevicesTextBox.AppendText(e.Data +"\n");
                            ADBDevicesTextBox.ScrollToEnd();
                        });
                    }
                };

                ProcessStartInfo fastbootPSI = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/C fastboot devices",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Process fastbootProcess = new Process
                {
                    StartInfo = fastbootPSI,
                    EnableRaisingEvents = true
                };

                fastbootProcess.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            FastbootDevicesTextBox.AppendText($"\n{e.Data}" + "\n");
                            FastbootDevicesTextBox.ScrollToEnd();
                        });
                    }
                };

                fastbootProcess.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            FastbootDevicesTextBox.AppendText("\n" + e.Data + "\n");
                            FastbootDevicesTextBox.ScrollToEnd();
                        });
                    }
                };

                adbProcess.Start();
                fastbootProcess.Start();
                adbProcess.BeginOutputReadLine();
                adbProcess.BeginErrorReadLine();
                fastbootProcess.BeginOutputReadLine();
                fastbootProcess.BeginErrorReadLine();
            }
            else
                MessageBox.Show("adb.exe Not found in path : " + adbpath);
        }

        private void ShellButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = null;

            if (ADBFirstDevice.IsChecked == true)
                selectedDevice = ADBFirstDevice.Content.ToString();
            else if (ADBSecondDevice.IsChecked == true)
                selectedDevice = ADBSecondDevice.Content.ToString();
            else if (ADBThirdDevice.IsChecked == true)
                selectedDevice = ADBThirdDevice.Content.ToString();

            if (selectedDevice == null)
                MessageBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                string command = $"adb -s {selectedDevice} shell";

                string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                if (File.Exists(Control))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/k {command}",
                        WorkingDirectory = adbpath,
                        UseShellExecute = true
                    };

                    Process.Start(psi);
                }
                else
                {
                    MessageBox.Show("ADB Not found: " + adbpath);
                }
            }
        }

        private void RebootButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            RebootOptions rebootOptions = new RebootOptions();

            rebootOptions.Owner = parent;
            rebootOptions.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            parent.Opacity = 0.4;
            rebootOptions.ShowDialog();
            parent.Opacity = 1.0;

            string selectedDevice = null;

            if (ADBFirstDevice.IsChecked == true)
                selectedDevice = ADBFirstDevice.Content.ToString();
            else if (ADBSecondDevice.IsChecked == true)
                selectedDevice = ADBSecondDevice.Content.ToString();
            else if (ADBThirdDevice.IsChecked == true)
                selectedDevice = ADBThirdDevice.Content.ToString();

            if (selectedDevice == null)
                MessageBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                if (rebootOptions.button_number == 0)
                {
                    string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"adb -s {selectedDevice} reboot recovery");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show($"{selectedDevice} Rebooting recovery using adb.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 1)
                {
                    string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"adb -s {selectedDevice} reboot bootloader");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show($"{selectedDevice} Rebooting bootloader using adb.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 2)
                {
                    string Control = System.IO.Path.Combine(adbpath, "adb.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"adb -s {selectedDevice} reboot system");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show($"{selectedDevice} Rebooting using adb.");
                    }
                    else
                    {
                        MessageBox.Show("ADB Not found: " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 3)
                {
                    rebootOptions.Close();
                }
            }
        }

        private void FlashButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            RebootOptions rebootOptions = new RebootOptions();

            rebootOptions.Owner = parent;
            rebootOptions.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            parent.Opacity = 0.4;
            rebootOptions.ShowDialog();
            parent.Opacity = 1.0;

            string selectedDevice = null;

            if (FastbootFirstDevice.IsChecked == true)
                selectedDevice = FastbootFirstDevice.Content.ToString();
            else if (FastbootSecondDevice.IsChecked == true)
                selectedDevice = FastbootSecondDevice.Content.ToString();
            else if (FastbootThirdDevice.IsChecked == true)
                selectedDevice = FastbootThirdDevice.Content.ToString();

                if (selectedDevice == null)
                MessageBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                if (rebootOptions.button_number == 0)
                {
                    OpenFileDialog image = new OpenFileDialog
                    {
                        Title = "Select a Recovery Image",
                        Filter = "Image (.img) - Compressed File (.zip)| *.img; *.zip",
                        Multiselect = false,
                        InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                    };
                    image.ShowDialog();

                    string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"fastboot -s {selectedDevice} flash recovery {image.FileName}");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show("Flashing recovery.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }

                }
                else if (rebootOptions.button_number == 1)
                {
                    OpenFileDialog image = new OpenFileDialog
                    {
                        Title = "Select a Bootloader Image",
                        Filter = "Image (.img) - Compressed File (.zip)| *.img; *.zip",
                        Multiselect = false,
                        InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                    };
                    image.ShowDialog();

                    string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"fastboot -s {selectedDevice} flash bootloader {image.FileName}");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show("Flashing bootloader.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 2)
                {
                    OpenFileDialog image = new OpenFileDialog
                    {
                        Title = "Select a System Image",
                        Filter = "Image (.img) - Compressed File (.zip)| *.img; *.zip",
                        Multiselect = false,
                        InitialDirectory = $@"C:\Users\{Environment.UserName}\Downloads"
                    };
                    image.ShowDialog();

                    string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"fastboot -s {selectedDevice} flash system {image.FileName}");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show("Flashing system.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 3)
                {
                    rebootOptions.Close();
                }
            }
        }

        private void FastbootRebootButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            RebootOptions rebootOptions = new RebootOptions();

            rebootOptions.Owner = parent;
            rebootOptions.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            parent.Opacity = 0.4;
            rebootOptions.ShowDialog();
            parent.Opacity = 1.0;

            string selectedDevice = null;

            if (FastbootFirstDevice.IsChecked == true)
                selectedDevice = FastbootFirstDevice.Content.ToString();
            else if (FastbootSecondDevice.IsChecked == true)
                selectedDevice = FastbootSecondDevice.Content.ToString();
            else if (FastbootThirdDevice.IsChecked == true)
                selectedDevice = FastbootThirdDevice.Content.ToString();

            if (selectedDevice == null)
                MessageBox.Show("CONNECT OR SELECT A DEVICE!");
            else
            {
                if (rebootOptions.button_number == 0)
                {

                    string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"fastboot -s {selectedDevice} reboot recovery");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show($"{selectedDevice} Rebooting recovery using fastboot.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 1)
                {

                    string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"fastboot -s {selectedDevice} reboot bootloader");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show($"{selectedDevice} Rebooting bootloader using fastboot.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 2)
                {
                    string Control = System.IO.Path.Combine(adbpath, "fastboot.exe");
                    if (File.Exists(Control))
                    {
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"fastboot -s {selectedDevice} reboot system");
                        });

                        th.IsBackground = true;
                        th.Start();

                        if (selectedDevice == "Device Not Found")
                            MessageBox.Show("CONNECT OR SELECT A DEVICE!");
                        else
                            MessageBox.Show($"{selectedDevice} Rebooting using fastboot.");
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
                    }
                }
                else if (rebootOptions.button_number == 3)
                {
                    rebootOptions.Close();
                }
            }
        }

        private void ProgramsButton_Click(object sender, RoutedEventArgs e)
        {
            Window parent = Window.GetWindow(this);
            ProgramsOptions rebootOptions = new ProgramsOptions();

            rebootOptions.Owner = parent;
            rebootOptions.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            parent.Opacity = 0.4;
            rebootOptions.ShowDialog();
            parent.Opacity = 1.0;
        }

        private void KillServerButton_Click(object sender, RoutedEventArgs e)
        {
            string Control = System.IO.Path.Combine(adbpath, "adb.exe");
            if (File.Exists(Control))
            {
                Thread th = new Thread(() =>
                {
                    string output = Commander($"adb kill-server");

                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Server(daemon) Killed!");
                    });
                });

                th.IsBackground = true;
                th.Start();
            }
            else
            {
                MessageBox.Show("adb.exe Not found in path : " + adbpath);
            }
        }

        private void StartServerButton_Click(object sender, RoutedEventArgs e)
        {
            string Control = System.IO.Path.Combine(adbpath, "adb.exe");
            if (File.Exists(Control))
            {
                Thread th = new Thread(() =>
                {
                    string output = Commander($"adb start-server");

                    Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Server(daemon) Started!");
                    });
                });

                th.IsBackground = true;
                th.Start();
            }
            else
            {
                MessageBox.Show("adb.exe Not found in path : " + adbpath);
            }
        }

        private void FilesButton_Click(object sender, RoutedEventArgs e)
        {
            FilesOptions fo = new FilesOptions();
            fo.Owner = this;
            Opacity = 0.4;
            fo.ShowDialog();
            Opacity = 1;
        }

        private void FastbootSidelodButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedDevice = null;

            if (ADBFirstDevice.IsChecked == true)
                selectedDevice = ADBFirstDevice.Content.ToString();
            else if (ADBSecondDevice.IsChecked == true)
                selectedDevice = ADBSecondDevice.Content.ToString();
            else if (ADBThirdDevice.IsChecked == true)
                selectedDevice = ADBThirdDevice.Content.ToString();

            if (selectedDevice == null)
                MessageBox.Show("CONNECT OR SELECT A DEVICE!");
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
                        Thread th = new Thread(() =>
                        {
                            string output = Commander($"adb -s {selectedDevice} sideload {dialog.FileName}");

                            Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show(output);
                            });
                        });

                        th.IsBackground = true;
                        th.Start();
                    }
                    else
                    {
                        MessageBox.Show("adb.exe Not found in path : " + adbpath);
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
    }
}
