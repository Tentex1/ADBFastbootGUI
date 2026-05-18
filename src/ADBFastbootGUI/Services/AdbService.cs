using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADBFastbootGUI.Services
{
    public class AdbService
    {
        private readonly string _adbPath;

        public AdbService(string adbPath = @".\")
        {
            _adbPath = adbPath;
        }

        public async Task<string> RunAdbCommandAsync(string arguments, bool useShellExecute = false, bool redirectOutput = true)
        {
            string adbExe = Path.Combine(_adbPath, "adb.exe");
            if (!File.Exists(adbExe))
            {
                throw new FileNotFoundException("adb.exe not found in path: " + _adbPath);
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = adbExe,
                Arguments = arguments,
                WorkingDirectory = _adbPath,
                UseShellExecute = useShellExecute,
                CreateNoWindow = true,
                RedirectStandardOutput = redirectOutput && !useShellExecute,
                RedirectStandardError = redirectOutput && !useShellExecute
            };

            using (Process process = new Process { StartInfo = psi })
            {
                process.Start();

                string output = "";
                if (redirectOutput && !useShellExecute)
                {
                    output = await process.StandardOutput.ReadToEndAsync();
                }

                await Task.Run(() => process.WaitForExit());
                return output;
            }
        }

        public async Task<List<string>> GetConnectedDevicesAsync()
        {
            try
            {
                string output = await RunAdbCommandAsync("devices");
                var lines = output.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                if (lines.Length <= 1)
                    return new List<string>();

                return lines.Skip(1)
                    .Where(line => line.Contains("device") && !line.Contains("List of devices"))
                    .Select(line => line.Split('\t')[0].Trim())
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<string> StartServerAsync()
        {
            return await RunAdbCommandAsync("start-server");
        }

        public async Task<string> KillServerAsync()
        {
            return await RunAdbCommandAsync("kill-server");
        }

        public async Task<string> RebootAsync(string deviceId, string mode = "")
        {
            string prefix = string.IsNullOrEmpty(deviceId) ? "" : $"-s {deviceId} ";
            string arg = string.IsNullOrEmpty(mode) ? "reboot" : $"reboot {mode}";
            return await RunAdbCommandAsync(prefix + arg);
        }
    }
}
