using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using ADBFastbootGUI.Services;

namespace ADBFastbootGUI.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly AdbService _adbService;
        private readonly FastbootService _fastbootService;

        private ObservableCollection<string> _adbDevices = new ObservableCollection<string>();
        public ObservableCollection<string> AdbDevices
        {
            get => _adbDevices;
            set => SetProperty(ref _adbDevices, value);
        }

        private ObservableCollection<string> _fastbootDevices = new ObservableCollection<string>();
        public ObservableCollection<string> FastbootDevices
        {
            get => _fastbootDevices;
            set => SetProperty(ref _fastbootDevices, value);
        }

        private string _selectedAdbDevice;
        public string SelectedAdbDevice
        {
            get => _selectedAdbDevice;
            set
            {
                if (SetProperty(ref _selectedAdbDevice, value))
                {
                    Task.Run(async () => await UpdateDeviceStatusAsync());
                }
            }
        }

        private string _selectedFastbootDevice;
        public string SelectedFastbootDevice
        {
            get => _selectedFastbootDevice;
            set
            {
                if (SetProperty(ref _selectedFastbootDevice, value))
                {
                    Task.Run(async () => await UpdateDeviceStatusAsync());
                }
            }
        }

        private string _deviceStatusText = "NO DEVICE DETECTED";
        public string DeviceStatusText
        {
            get => _deviceStatusText;
            set => SetProperty(ref _deviceStatusText, value);
        }

        private string _deviceStatusIcon = "AlertCircleOutline";
        public string DeviceStatusIcon
        {
            get => _deviceStatusIcon;
            set => SetProperty(ref _deviceStatusIcon, value);
        }

        private Brush _deviceStatusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828"));
        public Brush DeviceStatusBrush
        {
            get => _deviceStatusBrush;
            set => SetProperty(ref _deviceStatusBrush, value);
        }

        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand RefreshCommand { get; }

        public MainViewModel(AdbService adbService, FastbootService fastbootService)
        {
            _adbService = adbService ?? throw new ArgumentNullException(nameof(adbService));
            _fastbootService = fastbootService ?? throw new ArgumentNullException(nameof(fastbootService));

            RefreshCommand = new RelayCommand(async () => await RefreshDevicesAsync());
        }

        public async Task RefreshDevicesAsync()
        {
            if (IsRefreshing) return;
            IsRefreshing = true;

            try
            {
                // Fetch connected ADB and Fastboot devices on background thread
                var adbList = await _adbService.GetConnectedDevicesAsync();
                var fbList = await _fastbootService.GetConnectedDevicesAsync();
                
                // Marshal UI changes back to UI thread
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    string previousAdbSelected = SelectedAdbDevice;
                    AdbDevices.Clear();
                    foreach (var dev in adbList)
                    {
                        AdbDevices.Add(dev);
                    }

                    if (AdbDevices.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(previousAdbSelected) && AdbDevices.Contains(previousAdbSelected))
                            SelectedAdbDevice = previousAdbSelected;
                        else
                            SelectedAdbDevice = AdbDevices[0];
                    }
                    else
                    {
                        SelectedAdbDevice = null;
                    }

                    string previousFbSelected = SelectedFastbootDevice;
                    FastbootDevices.Clear();
                    foreach (var dev in fbList)
                    {
                        FastbootDevices.Add(dev);
                    }

                    if (FastbootDevices.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(previousFbSelected) && FastbootDevices.Contains(previousFbSelected))
                            SelectedFastbootDevice = previousFbSelected;
                        else
                            SelectedFastbootDevice = FastbootDevices[0];
                    }
                    else
                    {
                        SelectedFastbootDevice = null;
                    }
                });

                // Update device details
                await UpdateDeviceStatusAsync();
            }
            catch
            {
                // Silently ignore to keep UI resilient
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        public async Task UpdateDeviceStatusAsync()
        {
            try
            {
                if (!string.IsNullOrEmpty(SelectedAdbDevice))
                {
                    string deviceId = SelectedAdbDevice;
                    string model = await _adbService.RunAdbCommandAsync($"-s {deviceId} shell getprop ro.product.model");
                    model = model.Trim();
                    
                    if (string.IsNullOrEmpty(model) || model.Contains("error"))
                    {
                        model = "Android Device";
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        DeviceStatusText = $"{model} [{deviceId}]";
                        DeviceStatusIcon = "CellphoneCheck";
                        DeviceStatusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E7D32"));
                    });
                }
                else if (!string.IsNullOrEmpty(SelectedFastbootDevice))
                {
                    string deviceId = SelectedFastbootDevice;
                    string product = await _fastbootService.RunFastbootCommandAsync($"-s {deviceId} getvar product");
                    string model = "Fastboot Device";
                    
                    if (!string.IsNullOrEmpty(product) && product.Contains("product:"))
                    {
                        var lines = product.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                        var productLine = lines.FirstOrDefault(l => l.Contains("product:"));
                        if (productLine != null)
                        {
                            model = productLine.Replace("product:", "").Trim();
                        }
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        DeviceStatusText = $"FASTBOOT: {model} [{deviceId}]";
                        DeviceStatusIcon = "Flash";
                        DeviceStatusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EF6C00"));
                    });
                }
                else
                {
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        DeviceStatusText = "NO DEVICE DETECTED";
                        DeviceStatusIcon = "AlertCircleOutline";
                        DeviceStatusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828"));
                    });
                }
            }
            catch
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    DeviceStatusText = "ERROR DETECTING DEVICE";
                    DeviceStatusIcon = "AlertCircleOutline";
                    DeviceStatusBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C62828"));
                });
            }
        }
    }
}
