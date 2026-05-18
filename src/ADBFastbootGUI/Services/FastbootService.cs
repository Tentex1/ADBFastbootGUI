using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ADBFastbootGUI.Services
{
    public class FastbootService
    {
        private readonly string _adbPath;

        public FastbootService(string adbPath = @".\")
        {
            _adbPath = adbPath;
        }

        public async Task<string> RunFastbootCommandAsync(string arguments, bool useShellExecute = false, bool redirectOutput = true)
        {
            string fastbootExe = Path.Combine(_adbPath, "fastboot.exe");
            if (!File.Exists(fastbootExe))
            {
                throw new FileNotFoundException("fastboot.exe not found in path: " + _adbPath);
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = fastbootExe,
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
                string output = await RunFastbootCommandAsync("devices");
                var lines = output.Split(new[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
                
                return lines
                    .Where(line => line.Contains("fastboot"))
                    .Select(line => line.Split('\t')[0].Trim())
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();
            }
            catch
            {
                return new List<string>();
            }
        }

        public async Task<string> RebootAsync(string deviceId, string mode = "")
        {
            string prefix = string.IsNullOrEmpty(deviceId) ? "" : $"-s {deviceId} ";
            string arg = string.IsNullOrEmpty(mode) ? "reboot" : $"reboot {mode}";
            return await RunFastbootCommandAsync(prefix + arg);
        }
    }
}
