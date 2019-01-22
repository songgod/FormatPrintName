using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Registry
{
    class Program
    {
        static void RegistRename()
        {
            string registApp = Directory.GetCurrentDirectory() + "\\Rename.exe";
            if (!File.Exists(registApp))
                return;
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.CreateSubKey("*\\shell\\PrintRename");
            appkey.SetValue("", "米样重命名");
            appkey.SetValue("icon", registApp);
            RegistryKey cmdkey = appkey.CreateSubKey("command");
            cmdkey.SetValue("", registApp + " %1");
            rootKey.Close();
        }

        static void RegistSample()
        {
            string registApp = Directory.GetCurrentDirectory() + "\\RenameSample.exe";
            if (!File.Exists(registApp))
                return;
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.CreateSubKey("*\\shell\\PrintRenameSample");
            appkey.SetValue("", "小样重命名");
            appkey.SetValue("icon", registApp);
            RegistryKey cmdkey = appkey.CreateSubKey("command");
            cmdkey.SetValue("", registApp + " %1");
            rootKey.Close();
        }

        static void RegistArrange()
        {
            string registApp = Directory.GetCurrentDirectory() + "\\Arrange.exe";
            if (!File.Exists(registApp))
                return;
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.CreateSubKey("Directory\\shell\\ArrangePrint");
            appkey.SetValue("", "米样统计");
            appkey.SetValue("icon", registApp);
            RegistryKey cmdkey = appkey.CreateSubKey("command");
            cmdkey.SetValue("", registApp + " %1");
            rootKey.Close();
        }


        static void RegistArrangeSample()
        {
            string registApp = Directory.GetCurrentDirectory() + "\\ArrangeSample.exe";
            if (!File.Exists(registApp))
                return;
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.CreateSubKey("Directory\\shell\\ArrangePrintSample");
            appkey.SetValue("", "小样统计");
            appkey.SetValue("icon", registApp);
            RegistryKey cmdkey = appkey.CreateSubKey("command");
            cmdkey.SetValue("", registApp + " %1");
            rootKey.Close();
        }

        static void Main(string[] args)
        {
            RegistRename();
            RegistSample();
            RegistArrange();
            RegistArrangeSample();
        }
    }
}
