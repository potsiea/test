using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainerZwift
{
    class Program
    {
        static void Main(string[] args)
        {
            Rectangle screenRect = Screen.PrimaryScreen.WorkingArea;

            Process[] processes = Process.GetProcesses();
            Process zwiftProcess = processes.Where(p => p.MainWindowTitle == "Zwift").FirstOrDefault();
            Process trainerRoadProcess = processes.Where(p => p.MainWindowTitle == "TrainerRoad").FirstOrDefault();

            // Resize border for existing Zwift instance.  Needs to be launched separately for login
            if (zwiftProcess != null)
            {
                IntPtr zwiftProcessHandle = zwiftProcess.MainWindowHandle;

                Win32.RemoveBorder(zwiftProcessHandle);
                Win32.MoveWindow(zwiftProcessHandle, 0, 0, screenRect.Width, (int)(screenRect.Height * 0.80));
            }
            
            // We can launch TrainerRoad if it hasn't been already
            if (trainerRoadProcess == null)
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall");
                string location = FindByDisplayName(regKey, "TrainerRoad for Windows");
                if (!string.IsNullOrEmpty(location))
                {
                    Process.Start($"{location}\\TrainerRoad.Net.exe");
                }
            }
        }

        private static string FindByDisplayName(RegistryKey parentKey, string name)
        {
            string[] nameList = parentKey.GetSubKeyNames();
            for (int i = 0; i < nameList.Length; i++)
            {
                RegistryKey regKey = parentKey.OpenSubKey(nameList[i]);
                try
                {
                    if (regKey.GetValue("DisplayName").ToString() == name)
                    {
                        return regKey.GetValue("InstallLocation").ToString();
                    }
                }
                catch { }
            }
            return "";
        }
    }
}
