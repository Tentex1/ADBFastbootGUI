# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.0.0] - 2026-05-18

This is a major architectural overhaul and modernization release. The application has been transformed from a synchronous legacy structure into a highly responsive, asynchronous MVVM desktop application.

### Added
- **MVVM Infrastructure**: Custom lightweight `ViewModelBase` (implementing `INotifyPropertyChanged`) and asynchronous `RelayCommand` (implementing `ICommand`).
- **Asynchronous Services**: Dedicated non-blocking services `AdbService` and `FastbootService` to handle CLI executions on isolated background threads, completely eliminating UI freezing.
- **Premium Connection Capsule**: A centered macOS/Windows 11 inspired Titlebar status badge with dynamic, color-coded state indicators and icons:
  - 🔴 **Disconnected**: Red badge with an alert circle icon.
  - 🟢 **ADB Connected**: Green badge with a cellphone check icon showing live device model details.
  - ⚡ **Fastboot Connected**: Orange badge with a lightning bolt icon showing fastboot product info.
- **Live Device Querying**: Background asynchronous querying of the actual device model using `adb shell getprop ro.product.model` and product names in Fastboot mode.
- **Virtual Compatibility API**: Get-only backward-compatibility layer in `MainWindow.xaml.cs` to emulate legacy RadioButtons, preserving 100% functionality for all 15+ sub-windows without requiring any modifications to their codebases.
- **Automatic Resource Cleanup**: Window `OnClosing` hook coupled with `Environment.Exit(0)` to completely stop all threads and cleanly kill remaining background processes (`adb.exe`, `fastboot.exe`, `scrcpy.exe`).
- **Safety Warnings**: Warning triggers (`MessageBoxResult`) for untested logical partition operations (Create, Erase, Resize) to guard against accidental device bricks.

### Changed
- **Infinite Device Listing**: Removed the old hardcoded 5-device limit. Replaced legacy static RadioButtons with dynamically data-bound WPF `ListBox` controls.
- **Thread-Safety**: Marshaled all background collection updates safely back to the UI thread using the Dispatcher.

---

## [1.0.0] - 2026-05-10

### Added
- Initial release of ADB & Fastboot GUI.
- Basic visual wrapping for standard ADB and Fastboot commands.
- Support for installing apps, pushing/pulling files, and partition flashing.
- Screen mirroring integration using Scrcpy.
- Simple Settings and About dialog windows.
