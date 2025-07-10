# 📱 ADB & Fastboot GUI

A simple and user-friendly GUI for ADB and Fastboot operations.  
Manage your Android device without needing to use the command line!

---

## 🧩 Features

### 🔹 ADB Mode (when device is turned on)
- `Shell` – Send terminal commands to the device  
- `Reboot` – Reboot into system, bootloader, or recovery  
- `Apps` – Install, uninstall, or list installed apps  
- `Files` – Push/pull files to/from the device  
- `Start Server` – Start the ADB server  
- `Kill Server` – Kill the ADB server  

### ⚡ Fastboot Mode (when device is in Fastboot)
- `Flash` – Flash .img files or ROMs  
- `Reboot` – Reboot back to system  
- `Sideload` – Sideload OTA updates from recovery

---

## 🔄 Refreshing Device List (F5 Key)

If your device doesn’t show up after plugging it in:

> 📌 Just press **F5** to refresh the ADB and Fastboot device list.  
> The interface will rescan and show all connected devices.

---

## ⚠️ Known Bug

- If you **hold down the F5 key** or **connect a device via USB**, the device list area might **duplicate or "clone" text**.
- This is only a visual glitch — nothing serious.
- ✅ **Solution:** Press **F5 once** to refresh the UI and clear the issue.

---

## 🛠 Requirements

- USB Debugging must be enabled on your Android device  
- USB drivers must be properly installed on your PC  
- Currently supported only on **Windows**

---

## 📷 Screenshot

> ![UI Screenshot](./screenshot.png)

---

## 📄 License

MIT License

---

## 💬 Contributing
Feel free to open an issue or request for feedback, improvements, or bug reports. 
If you find it helpful, you can leave a ⭐ or [Donate](https://buymeacoffee.com/duranforreal) to support the project.
