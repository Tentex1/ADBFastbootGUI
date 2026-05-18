# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [2.0.4] - 2026-05-18

This is a major release featuring a robust architectural overhaul, dynamic UI enhancements, performance optimizations, and critical bug fixes. The application has been fully modernized into a highly responsive, asynchronous MVVM desktop application.

### Added
- **Modern MVVM Architecture**: Custom lightweight `ViewModelBase` and asynchronous `RelayCommand` implementation.
- **Asynchronous Services**: Dedicated background services (`AdbService` and `FastbootService`) preventing UI thread freezes.
- **Dynamic Connection Capsule**: A macOS/Windows 11 inspired Titlebar status badge with dynamic, color-coded state indicators and icons:
  - 🔴 **Disconnected**: Red badge with an alert circle icon.
  - 🟢 **ADB Connected**: Green badge with a cellphone check icon showing live device model details.
  - ⚡ **Fastboot Connected**: Orange badge with a lightning bolt icon showing fastboot product info.
- **Scrcpy HD Display Screen Mirroring**: Advanced high-definition screen mirroring with custom video codecs, audio controls, and recording options.
- **Advanced APK Installer and Packages Uninstaller**: Enhanced user application and package management.
- **Multi-Device Connection Filters (Fastboot & ADB)**: Infinite dynamic device list with active connection filters, replacing the old 5-device limit.
- **Dark Mode UI and Custom Theme Configurations**: Premium dark/light themes and accent colors.
- **OEM Lock/Unlock & System Restore Logs**: Direct access to bootloader lock management and flashing status outputs.
- **Virtual Compatibility API**: Get-only backward-compatibility layer in `MainWindow.xaml.cs` to emulate legacy RadioButtons, preserving 100% functionality for all 15+ sub-windows.
- **Automatic Resource Cleanup**: Clean terminate processes (`adb`, `fastboot`, `scrcpy`) on window closing using `Environment.Exit(0)`.
- **Safety Warnings**: Warning triggers (`MessageBoxResult`) for untested logical partition operations to guard against accidental device bricks.

### Fixed
- **Fixed Bug Causing APK Crash on Older Models**: Resolved threading and deployment crashes when dealing with legacy Android versions.

---

## [1.0.0] - 2026-05-10

### Added
- Initial release of ADB & Fastboot GUI.
- Basic visual wrapping for standard ADB and Fastboot commands.
- Support for installing apps, pushing/pulling files, and partition flashing.
- Screen mirroring integration using Scrcpy.
- Simple Settings and About dialog windows.
