/* ==========================================================================
   ADB & Fastboot GUI - Native C# WPF Replica Interactive Mechanics (script.js)
   ========================================================================== */

document.addEventListener('DOMContentLoaded', () => {

    /* ==========================================================================
       1. Light / Dark Theme Switcher
       ========================================================================== */
    const themeToggleBtn = document.getElementById('theme-toggle');
    const htmlElement = document.documentElement;
    const themeIcon = themeToggleBtn.querySelector('i');

    const savedTheme = localStorage.getItem('site-theme') ||
        (window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light');

    htmlElement.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);

    themeToggleBtn.addEventListener('click', () => {
        const currentTheme = htmlElement.getAttribute('data-theme');
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';

        htmlElement.setAttribute('data-theme', newTheme);
        localStorage.setItem('site-theme', newTheme);
        updateThemeIcon(newTheme);

        appendSimTerminalLog('ui-theme', `MaterialDesign theme swapped: ${newTheme.toUpperCase()}`);
    });

    function updateThemeIcon(theme) {
        if (theme === 'dark') {
            themeIcon.className = 'fas fa-sun';
            themeToggleBtn.title = 'Switch to Light Mode';
        } else {
            themeIcon.className = 'fas fa-moon';
            themeToggleBtn.title = 'Switch to Dark Mode';
        }
    }


    /* ==========================================================================
       2. Sticky Header Scroll Effect
       ========================================================================== */
    const headerElement = document.getElementById('site-header');
    window.addEventListener('scroll', () => {
        if (window.scrollY > 20) {
            headerElement.classList.add('scrolled');
        } else {
            headerElement.classList.remove('scrolled');
        }
    });


    /* ==========================================================================
       3. WPF-Style Desktop App Mockup Simulator
       ========================================================================== */

    // Core Elements
    const wpfTabButtons = document.querySelectorAll('.wpf-tab-btn');
    const wpfTabContents = document.querySelectorAll('.wpf-tab-content');
    const simDeviceStatus = document.getElementById('sim-device-status');
    const simTerminalScreen = document.getElementById('sim-terminal-screen');
    const simStatusCapsule = document.getElementById('sim-status-capsule');
    const simStatusCapsuleText = document.getElementById('sim-status-capsule-text');

    let activeSerial = '987654321A88X';
    let activeModel = 'Pixel 8 Pro';
    let isDeviceConnected = true;
    let logcatInterval = null;

    // WPF Custom Confirmation MessageBox State
    let pendingWpfAction = null;

    const showWpfMessageBox = (message, title, onConfirm) => {
        const backdrop = document.getElementById('sim-mb-backdrop');
        const textEl = document.getElementById('sim-mb-text');
        const headerEl = backdrop.querySelector('.wpf-mb-header span');

        headerEl.textContent = title || 'Confirm Logical Gate Operation';
        textEl.textContent = message;
        backdrop.style.display = 'flex';

        pendingWpfAction = onConfirm;
    };

    const closeWpfMessageBox = () => {
        document.getElementById('sim-mb-backdrop').style.display = 'none';
        pendingWpfAction = null;
    };

    document.getElementById('sim-mb-btn-yes').addEventListener('click', () => {
        if (pendingWpfAction) {
            pendingWpfAction();
        }
        closeWpfMessageBox();
    });

    document.getElementById('sim-mb-btn-no').addEventListener('click', () => {
        appendSimTerminalLog('MessageBoxResult', 'MessageBoxResult.No - Operation aborted safely by user gate.');
        closeWpfMessageBox();
    });

    document.getElementById('sim-mb-close-btn').addEventListener('click', () => {
        appendSimTerminalLog('MessageBoxResult', 'MessageBoxResult.Cancel - Dialog closed.');
        closeWpfMessageBox();
    });

    // Asynchronous Device Query Simulation
    const triggerDeviceQuery = (deviceName, serial, isConnected = true) => {
        if (!simStatusCapsule) return;

        simStatusCapsule.className = 'device-status-capsule querying';
        simStatusCapsuleText.textContent = 'Querying device...';
        appendSimTerminalLog('BackgroundWorker', `[Thread-5] Querying device properties asynchronously...`);

        setTimeout(() => {
            if (isConnected) {
                simStatusCapsule.className = 'device-status-capsule';
                simStatusCapsuleText.textContent = `${deviceName} (Active)`;
                simDeviceStatus.textContent = `MONITORING - ${deviceName} Connected`;
                simDeviceStatus.style.color = '#10b981';
                simDeviceStatus.style.borderColor = 'rgba(16, 185, 129, 0.3)';
                activeModel = deviceName;
                activeSerial = serial;
                isDeviceConnected = true;
                appendSimTerminalLog('BackgroundWorker', `[Thread-5] Device properties resolved: ${deviceName} [${serial}] at 60+ FPS.`);
            } else {
                simStatusCapsule.className = 'device-status-capsule disconnected';
                simStatusCapsuleText.textContent = 'Disconnected';
                simDeviceStatus.textContent = 'Disconnected';
                simDeviceStatus.style.color = '#ef4444';
                simDeviceStatus.style.borderColor = 'rgba(239, 68, 68, 0.3)';
                isDeviceConnected = false;
                appendSimTerminalLog('BackgroundWorker', `[Thread-5] Query finished: No active devices attached.`);
            }
        }, 600);
    };

    if (simStatusCapsule) {
        simStatusCapsule.addEventListener('click', () => {
            if (isDeviceConnected) {
                triggerDeviceQuery(activeModel, activeSerial, true);
            } else {
                triggerDeviceQuery('Pixel 8 Pro', '987654321A88X', true);
            }
        });
    }

    // A. WPF Tab Navigation Control
    wpfTabButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const targetTab = btn.getAttribute('data-tab');

            // Swap active button states
            wpfTabButtons.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            // Swap active content containers
            wpfTabContents.forEach(content => {
                content.classList.remove('active');
                if (content.id === `sim-tab-${targetTab}`) {
                    content.classList.add('active');
                }
            });

            appendSimTerminalLog('wpf-navigation', `TabSelectionChanged: Selected index ${targetTab.toUpperCase()}`);
        });
    });

    // B. Title bar action bindings
    document.getElementById('sim-about-menu').addEventListener('click', () => {
        appendSimTerminalLog('AboutMenu', 'ADB and Fastboot GUI v2.0 - Developed by Tentex1 under MIT license.');
    });

    document.getElementById('sim-btn-settings').addEventListener('click', () => {
        appendSimTerminalLog('SettingsWindow', 'MainWindow.xaml -> Loaded SettingsDialog.xaml modal successfully.');
    });

    document.getElementById('sim-btn-minimize').addEventListener('click', () => {
        appendSimTerminalLog('WindowAction', 'MainWindow State minimized.');
    });

    document.getElementById('sim-btn-close').addEventListener('click', () => {
        appendSimTerminalLog('ExitApplication', 'Shutdown close request received.');
        appendSimTerminalLog('ProcessSweeper', '[Sweeper] Closing active CLI execution threads...');
        appendSimTerminalLog('ProcessSweeper', '[Sweeper] Sending SIGTERM to lingering adb.exe (PID 5037)... Success.');
        appendSimTerminalLog('ProcessSweeper', '[Sweeper] Sending SIGTERM to lingering scrcpy.exe... Success.');
        appendSimTerminalLog('ProcessSweeper', '[Sweeper] Hooking native Environment.Exit(0) call...');

        simStatusCapsule.className = 'device-status-capsule disconnected';
        simStatusCapsuleText.textContent = 'Disconnected';
        simDeviceStatus.textContent = 'Disconnected';
        simDeviceStatus.style.color = '#ef4444';
        simDeviceStatus.style.borderColor = 'rgba(239, 68, 68, 0.3)';
        isDeviceConnected = false;

        setTimeout(() => {
            appendSimTerminalLog('ApplicationStarted', 'Reloading main process window...');
            triggerDeviceQuery('Pixel 8 Pro', '987654321A88X', true);
        }, 3500);
    });

    // C. Simulated Button Clicks - General Router
    const actionButtons = document.querySelectorAll('[data-action]');
    actionButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const action = btn.getAttribute('data-action');
            handleSimulatedAction(action);
        });
    });

    function handleSimulatedAction(action) {
        if (!isDeviceConnected && action !== 'start-server') {
            appendSimTerminalLog('adb-error', 'error: no devices/emulators found');
            return;
        }

        switch (action) {
            case 'start-server':
                appendSimTerminalLog('adb start-server', 'Starting ADB server daemon on local port 5037...');
                setTimeout(() => {
                    appendSimTerminalLog('adb-daemon', '* daemon started successfully *');
                    simDeviceStatus.textContent = 'ADB Server Active';
                    simDeviceStatus.style.color = '#10b981';
                    simDeviceStatus.style.borderColor = 'rgba(16, 185, 129, 0.3)';
                    isDeviceConnected = true;
                }, 300);
                break;
            case 'kill-server':
                appendSimTerminalLog('adb kill-server', 'Closing daemon socket connection...');
                simDeviceStatus.textContent = 'Server Offline';
                simDeviceStatus.style.color = '#ef4444';
                simDeviceStatus.style.borderColor = 'rgba(239, 68, 68, 0.3)';
                isDeviceConnected = false;
                break;
            case 'reboot-system':
                appendSimTerminalLog('adb reboot', 'Rebooting device to Android OS...');
                simulateHardwareOff('Rebooting...');
                break;
            case 'reboot-bootloader':
                appendSimTerminalLog('adb reboot bootloader', 'Rebooting device into Bootloader interface...');
                simulateHardwareOff('Fastboot Mode');
                break;
            case 'reboot-recovery':
                appendSimTerminalLog('adb reboot recovery', 'Rebooting device into Android System Recovery...');
                simulateHardwareOff('Recovery Mode');
                break;
            case 'reboot-recovery-fb':
                appendSimTerminalLog('fastboot reboot recovery', 'Instructing fastboot kernel to reboot recovery...');
                simulateHardwareOff('Recovery Mode');
                break;
            case 'reboot-system-fb':
                appendSimTerminalLog('fastboot reboot', 'Rebooting bootloader core to system OS...');
                simulateHardwareOff('Rebooting...');
                break;
            case 'reboot-bootloader-fb':
                appendSimTerminalLog('fastboot reboot-bootloader', 'Resetting fastboot protocol loop...');
                break;
            case 'reboot-image-fb':
                appendSimTerminalLog('fastboot boot image.img', 'Booting dynamic modular image chunk temporary...');
                break;
            case 'oem-lock':
                appendSimTerminalLog('fastboot oem lock', 'Verifying security signature key... Locked bootloader.');
                break;
            case 'oem-unlock':
                appendSimTerminalLog('fastboot oem unlock', 'Warning: unlocking system wipes userdata. Unlocked bootloader.');
                break;
            case 'oem-info':
                appendSimTerminalLog('fastboot getvar all', 'OEM diagnostic values loaded:\n(bootloader) secure: yes\n(bootloader) version-bootloader: 3.01');
                break;
            case 'adb-shell':
                appendSimTerminalLog('adb shell', `adb shell connected to device [${activeSerial}]`);
                appendSimTerminalLog('shell-input', 'shell@android:/ $ whoami\nshell\nshell@android:/ $ ');
                break;
            case 'sideload':
                appendSimTerminalLog('adb sideload dynamic_ota.zip', 'Verifying package metadata... Sideloading started.');
                simulateProgressFlash('OTA Update');
                break;
        }
    }

    function simulateHardwareOff(temporaryStatus) {
        simDeviceStatus.textContent = temporaryStatus;
        simDeviceStatus.style.color = '#f59e0b';
        simDeviceStatus.style.borderColor = 'rgba(245, 158, 11, 0.3)';

        // Reconnect after 3 seconds
        setTimeout(() => {
            simDeviceStatus.textContent = 'Pixel 8 Connected';
            simDeviceStatus.style.color = '#10b981';
            simDeviceStatus.style.borderColor = 'rgba(16, 185, 129, 0.3)';
            appendSimTerminalLog('adb-daemon', `Device attached: ${activeSerial}`);
        }, 3000);
    }

    // D. Simulated Wireless Connection Prompts
    const btnWireless = document.getElementById('sim-btn-wireless-connect');
    const btnWirelessDev = document.getElementById('sim-btn-wireless-connect-dev');

    const handleWirelessFlow = () => {
        appendSimTerminalLog('adb connect', 'Requesting TCP/IP pairing parameters...');
        const ipInput = prompt('Enter Android wireless debugging IP Address & Port (e.g. 192.168.1.100:5555):', '192.168.1.100:5555');
        if (ipInput) {
            const pairCode = prompt('Enter 6-digit wireless pairing code (PIN):', '123456');
            if (pairCode) {
                appendSimTerminalLog(`adb pair ${ipInput} ${pairCode}`, 'Verifying dynamic pairing keys authorization...');
                setTimeout(() => {
                    appendSimTerminalLog('adb connect output', `Successfully paired and connected to wireless node: ${ipInput}`);
                    simDeviceStatus.textContent = 'Wi-Fi Attached';
                }, 800);
            }
        }
    };

    if (btnWireless) btnWireless.addEventListener('click', handleWirelessFlow);
    if (btnWirelessDev) btnWirelessDev.addEventListener('click', handleWirelessFlow);

    // E. ADB Tab App Managers
    document.getElementById('sim-btn-app-install').addEventListener('click', () => {
        appendSimTerminalLog('adb install app.apk', 'Pushing APK payload to partition storage... Success.');
    });

    document.getElementById('sim-btn-app-uninstall').addEventListener('click', () => {
        const pkg = prompt('Enter Android app package name to uninstall:', 'com.android.chrome');
        if (pkg) {
            appendSimTerminalLog(`adb uninstall ${pkg}`, `Requesting package list removal of [${pkg}]...`);
            setTimeout(() => {
                appendSimTerminalLog('uninstall-output', 'Success');
            }, 600);
        }
    });

    document.getElementById('sim-btn-app-list').addEventListener('click', () => {
        appendSimTerminalLog('adb shell pm list packages -3', 'Third-Party packages found:\npackage:com.whatsapp\npackage:com.instagram.android\npackage:org.mozilla.firefox');
    });

    document.getElementById('sim-btn-send-file').addEventListener('click', () => {
        appendSimTerminalLog('adb push backup.tar /sdcard/', 'Uploading backup archive... [48 MB/s] finished.');
    });

    document.getElementById('sim-btn-get-device-info').addEventListener('click', () => {
        appendSimTerminalLog('adb shell getprop ro.product.model', 'ro.product.model: Pixel 8 Pro');
        appendSimTerminalLog('adb shell getprop ro.serialno', `ro.serialno: ${activeSerial}`);
    });

    // F. Logcat Real-Time Logger Stream Toggle
    const btnRealtimeLog = document.getElementById('sim-btn-realtime-log');
    btnRealtimeLog.addEventListener('click', () => {
        if (logcatInterval) {
            // Stop logging
            clearInterval(logcatInterval);
            logcatInterval = null;
            btnRealtimeLog.textContent = 'Real-Time Log';
            btnRealtimeLog.classList.remove('wpf-btn-danger');
            appendSimTerminalLog('logcat', 'Logcat output capture terminated by user.');
        } else {
            // Start logging
            btnRealtimeLog.textContent = 'Stop Log';
            btnRealtimeLog.classList.add('wpf-btn-danger');
            appendSimTerminalLog('adb logcat', 'Starting real-time logcat capture stream...');

            const logEntries = [
                'I/ActivityManager: Start proc com.google.android.youtube for activity',
                'D/WifiService: acquireWifiLockLocked: WifiLock{background}',
                'I/PowerManagerService: Going to sleep due to power button click...',
                'V/Sensors: SensorManager event callback index 2',
                'W/AudioService: Volume control blocked by security permission check'
            ];

            logcatInterval = setInterval(() => {
                const randLog = logEntries[Math.floor(Math.random() * logEntries.length)];
                appendSimTerminalLog('logcat-stream', `05-18 18:40:02.102 [LOG] ${randLog}`);
            }, 700);
        }
    });

    // G. Fastboot Flash & Erase Button Click Loops with Safe Warning Gates
    const flashButtons = document.querySelectorAll('[data-flash]');
    flashButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const partName = btn.getAttribute('data-flash');
            const message = `Are you sure you want to FLASH the '${partName.toUpperCase()}' partition? This writes raw binary blocks on device storage. Make sure your active target device slot is selected.`;

            showWpfMessageBox(message, `Warning: Writing raw binary blocks`, () => {
                appendSimTerminalLog(`fastboot flash ${partName.toLowerCase()} ${partName.toLowerCase()}.img`, `[BackgroundThread-4] Sending flash chunks data package '${partName}'...`);
                simulateProgressFlash(partName);
            });
        });
    });

    const eraseButtons = document.querySelectorAll('[data-erase]');
    eraseButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            const partName = btn.getAttribute('data-erase');
            const message = `Danger: Erasing logical block allocations. Are you sure you want to ERASE the '${partName.toUpperCase()}' partition? This completely wipes raw data allocations.`;

            showWpfMessageBox(message, `Warning: Erase Raw Flash Allocation`, () => {
                appendSimTerminalLog(`fastboot erase ${partName.toLowerCase()}`, `[BackgroundThread-4] Wiping raw flash allocation block: '${partName}'...`);
                setTimeout(() => {
                    appendSimTerminalLog('fastboot-status', `[BackgroundThread-4] erase '${partName}': OK. [0.06s]`);

                    // Reboot option check
                    const shouldReboot = document.getElementById('sim-opt-reboot').checked;
                    if (shouldReboot) {
                        appendSimTerminalLog('fastboot reboot', 'Reboot-Immediate setting active. Power cycling device...');
                        simulateHardwareOff('Rebooting...');
                    }
                }, 600);
            });
        });
    });

    function simulateProgressFlash(partitionName) {
        let pct = 0;
        appendSimTerminalLog('flash-transfer', `Writing partition '${partitionName}': 0%`);

        const interval = setInterval(() => {
            pct += 25;
            appendSimTerminalLog('flash-transfer', `Writing partition '${partitionName}': ${pct}%`);

            if (pct >= 100) {
                clearInterval(interval);
                setTimeout(() => {
                    appendSimTerminalLog('flash-status', `Finished writing '${partitionName}' partition successfully. (1.04s)`);

                    // Check reboot options in Fastboot tab
                    const shouldReboot = document.getElementById('sim-opt-reboot').checked;
                    if (shouldReboot && partitionName !== 'OTA Update') {
                        appendSimTerminalLog('fastboot reboot', 'Reboot-Immediate setting active. Power cycling device...');
                        simulateHardwareOff('Rebooting...');
                    }
                }, 200);
            }
        }, 250);
    }

    // H. Advanced DSU & Partitions Management with Confirmation Gates
    document.getElementById('sim-btn-dsu-enable').addEventListener('click', () => {
        appendSimTerminalLog('adb shell setprop persist.sys.fflag.device_states DSU', 'DSU Loader features verified and enabled.');
    });
    document.getElementById('sim-btn-dsu-disable').addEventListener('click', () => {
        appendSimTerminalLog('adb shell setprop persist.sys.fflag.device_states null', 'DSU Loader features disabled.');
    });
    document.getElementById('sim-btn-part-create').addEventListener('click', () => {
        const msg = "Logical Partition Alert: Are you sure you want to allocate a new logical system partition 'system_b' inside dynamic super storage blocks?";
        showWpfMessageBox(msg, "Create Dynamic Partition", () => {
            appendSimTerminalLog('fastboot create-logical-partition system_b 1024', '[BackgroundThread-2] Logical system_b partition block created.');
        });
    });
    document.getElementById('sim-btn-part-erase').addEventListener('click', () => {
        const msg = "Danger: Are you sure you want to delete the active dynamic logical 'userdata_b' partition block? This operation cannot be undone.";
        showWpfMessageBox(msg, "Erase Dynamic User Partition", () => {
            appendSimTerminalLog('fastboot delete-logical-partition userdata_b', '[BackgroundThread-2] Erase userdata_b logical partition block: Success.');
        });
    });
    document.getElementById('sim-btn-part-resize').addEventListener('click', () => {
        const msg = "Resize Boundary Warning: Are you sure you want to modify logical partition boundary parameters for 'system_a'?";
        showWpfMessageBox(msg, "Resize Dynamic Boundary", () => {
            appendSimTerminalLog('fastboot resize-logical-partition system_a 2048', '[BackgroundThread-2] Resized system_a layout structure boundaries.');
        });
    });

    // I. Devices Radio buttons active serial changer with dynamic asynchronous model query
    const deviceRadioList = document.querySelectorAll('.wpf-device-list input');
    deviceRadioList.forEach(radio => {
        radio.addEventListener('change', () => {
            const radioWrapper = radio.closest('.wpf-radio');
            const deviceName = radioWrapper.getAttribute('data-device-name');
            const deviceSerial = radioWrapper.getAttribute('data-device-serial');

            if (deviceName && deviceSerial) {
                appendSimTerminalLog('DeviceSelect', `Connection request initiated for ${deviceName} [${deviceSerial}]`);
                triggerDeviceQuery(deviceName, deviceSerial, true);
            }
        });
    });

    // Device search filter manager for unlimited devices
    const deviceSearchInput = document.getElementById('sim-device-search');
    if (deviceSearchInput) {
        deviceSearchInput.addEventListener('input', () => {
            const query = deviceSearchInput.value.toLowerCase().trim();
            const adbLabels = document.querySelectorAll('#sim-adb-device-list .wpf-radio');
            const fbLabels = document.querySelectorAll('#sim-fb-device-list .wpf-radio');

            let matchCount = 0;

            const filterList = (labels) => {
                labels.forEach(label => {
                    const name = label.getAttribute('data-device-name').toLowerCase();
                    const serial = label.getAttribute('data-device-serial').toLowerCase();
                    if (name.includes(query) || serial.includes(query)) {
                        label.style.display = 'inline-flex';
                        matchCount++;
                    } else {
                        label.style.display = 'none';
                    }
                });
            };

            filterList(adbLabels);
            filterList(fbLabels);

            appendSimTerminalLog('DeviceManager', `[DeviceFilter] Filtered active connections using criteria: "${query}" (${matchCount} visible)`);
        });
    }

    // J. Scrcpy Actions & Mirroring Float overlay window toggles
    const wpfScrcpyOverlay = document.getElementById('wpf-scrcpy-mirror-frame');
    const simPhoneMockup = document.getElementById('sim-phone-mockup');
    const phoneStatusText = simPhoneMockup.querySelector('.phone-status-text');

    const btnStartMirror = document.getElementById('sim-btn-start-mirror');
    const btnStartMirrorQuick = document.getElementById('sim-btn-toggle-mirror');
    const overlayCloseBtn = document.getElementById('sim-btn-close-scrcpy-overlay');

    const openScrcpyCasting = () => {
        const item = document.getElementById('sim-combo-mirror-item').value;
        appendSimTerminalLog('scrcpy', `Opening projection stream overlay frame for [${item}]...`);

        // Show Floating overlay window
        wpfScrcpyOverlay.style.display = 'flex';
        simPhoneMockup.classList.add('active');
        phoneStatusText.textContent = 'SCREEN CAST';

        appendSimTerminalLog('scrcpy-status', 'Casting frame buffers active. Resolution matches screen aspect.');
    };

    if (btnStartMirror) btnStartMirror.addEventListener('click', openScrcpyCasting);
    if (btnStartMirrorQuick) btnStartMirrorQuick.addEventListener('click', openScrcpyCasting);

    const closeScrcpyCasting = () => {
        wpfScrcpyOverlay.style.display = 'none';
        simPhoneMockup.classList.remove('active');
        phoneStatusText.textContent = 'DISCONNECTED';
        appendSimTerminalLog('scrcpy', 'Casting projection frame disconnected.');
    };

    if (overlayCloseBtn) overlayCloseBtn.addEventListener('click', closeScrcpyCasting);

    document.getElementById('sim-btn-start-record').addEventListener('click', () => {
        const format = document.getElementById('sim-combo-record-format').value;
        appendSimTerminalLog('scrcpy --record', `Starting screen recording directly in output format: ${format}`);
        appendSimTerminalLog('scrcpy-record-status', 'Recording active... (Press Stop Recording inside App to save)');
    });

    document.getElementById('sim-btn-start-otg').addEventListener('click', () => {
        const opt = document.getElementById('sim-combo-otg-opt').value;
        appendSimTerminalLog('scrcpy --otg', `Initializing PC Keyboard / Gamepad relay mode [${opt}]... Success.`);
    });


    // K. General Logging Core Screen Appender
    function appendSimTerminalLog(command, output) {
        const cursor = simTerminalScreen.querySelector('.terminal-cursor');
        if (cursor) cursor.remove();

        const isShellCommand = !command.includes('output') && !command.includes('status') && !command.includes('daemon') && !command.includes('logcat') && !command.includes('Select') && !command.includes('Window') && !command.includes('Settings') && !command.includes('About') && !command.includes('navigation');
        const cmdPrefix = isShellCommand ? '$ ' : '';

        const lineContainer = document.createElement('div');
        lineContainer.className = 'terminal-line';

        if (isShellCommand) {
            lineContainer.innerHTML = `<span class="command">${cmdPrefix}${command}</span>`;
            simTerminalScreen.appendChild(lineContainer);

            const respContainer = document.createElement('div');
            respContainer.className = 'terminal-line';
            respContainer.innerHTML = `<span class="info">${output}</span>`;
            simTerminalScreen.appendChild(respContainer);
        } else {
            lineContainer.innerHTML = `<span class="info">${output}</span>`;
            simTerminalScreen.appendChild(lineContainer);
        }

        // Re-append blinking cursor
        const newCursor = document.createElement('span');
        newCursor.className = 'terminal-cursor';
        simTerminalScreen.appendChild(newCursor);

        // Auto-scroll screen
        simTerminalScreen.scrollTop = simTerminalScreen.scrollHeight;
    }


    /* ==========================================================================
       4. Connection Guides Interactive Tab Panel Swapper
       ========================================================================== */
    const guideTabBtns = document.querySelectorAll('.guide-tab-btn');
    const guideContentPanels = document.querySelectorAll('.guide-content-panel');

    guideTabBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            const targetGuide = btn.getAttribute('data-guide');

            guideTabBtns.forEach(b => b.classList.remove('active'));
            btn.classList.add('active');

            guideContentPanels.forEach(panel => {
                panel.classList.remove('active');
                if (panel.id === `guide-${targetGuide}`) {
                    panel.classList.add('active');
                }
            });

            appendSimTerminalLog('ui-guide', `Selected ConnectionGuide panel: ${targetGuide.toUpperCase()}`);
        });
    });


    /* ==========================================================================
       5. Developer CLI Quickstart Code Block Copy Action
       ========================================================================== */
    const btnCopyCli = document.getElementById('btn-copy-cli');
    const cliCodeBlock = document.getElementById('cli-code-block');

    btnCopyCli.addEventListener('click', () => {
        const tempElement = document.createElement('div');
        tempElement.innerHTML = cliCodeBlock.innerHTML;
        const codeText = tempElement.innerText;

        navigator.clipboard.writeText(codeText).then(() => {
            btnCopyCli.innerHTML = '<i class="fas fa-check"></i> Copied!';
            btnCopyCli.style.background = 'rgba(16, 185, 129, 0.15)';
            btnCopyCli.style.color = '#10b981';
            btnCopyCli.style.borderColor = 'rgba(16, 185, 129, 0.3)';

            setTimeout(() => {
                btnCopyCli.innerHTML = '<i class="fas fa-copy"></i> Copy';
                btnCopyCli.style.background = 'var(--primary-glow)';
                btnCopyCli.style.color = 'var(--primary)';
                btnCopyCli.style.borderColor = 'rgba(var(--primary-rgb), 0.3)';
            }, 2000);

            appendSimTerminalLog('clipboard', 'Quickstart developer command variables cached to system clipboard.');
        }).catch(err => {
            console.error('Failed to copy command scripts: ', err);
        });
    });


    /* ==========================================================================
       6. Premium FAQ Collapsible Accordions Mechanics
       ========================================================================== */
    const faqItems = document.querySelectorAll('.faq-item');

    faqItems.forEach(item => {
        const header = item.querySelector('.faq-header');
        const body = item.querySelector('.faq-body');

        header.addEventListener('click', () => {
            const isCurrentlyActive = item.classList.contains('active');

            faqItems.forEach(otherItem => {
                otherItem.querySelector('.faq-body').style.maxHeight = null;
            });

            if (!isCurrentlyActive) {
                item.classList.add('active');
                body.style.maxHeight = body.scrollHeight + 'px';
                appendSimTerminalLog('ui-faq', 'Expanded accordion item question details.');
            } else {
                item.classList.remove('active');
                body.style.maxHeight = null;
            }
        });
    });


    /* ==========================================================================
       7. Scroll-Reveal Viewport Animations (Intersection Observer)
       ========================================================================== */
    const revealElements = document.querySelectorAll('.reveal-element');

    const revealObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('active');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    });

    revealElements.forEach(el => {
        revealObserver.observe(el);
    });

});