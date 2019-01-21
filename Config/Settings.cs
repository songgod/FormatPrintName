using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config
{
    static public class Settings
    {
        static Settings()
        {
            lstFormat = new List<string>();
            lstFaceName = new List<string>();
            LoadFromFile();
        }
        static public List<string> lstFormat { get; set; }
        static public List<string> lstFaceName { get; set; }

        static public void LoadFromFile()
        {
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.OpenSubKey("*\\shell\\PrintRename");
            string url = appkey.GetValue("icon") as string;
            if (string.IsNullOrWhiteSpace(url))
                return;
            string fmturl = Path.GetDirectoryName(url) + "\\format.txt";
            string fnurl = Path.GetDirectoryName(url) + "\\facename.txt";
            lstFormat = ReadFile(fmturl);
            lstFaceName = ReadFile(fnurl);
        }

        static private List<string> ReadFile(string file)
        {
            List<string> res = new List<string>();
            try
            {
                StreamReader reader = new StreamReader(file, Encoding.Default);
                if (reader == null)
                    return res;

                string l = reader.ReadLine();
                while (!string.IsNullOrWhiteSpace(l))
                {
                    res.Add(l);
                    l = reader.ReadLine();
                }
                reader.Close();
                return res;
            }
            catch (Exception)
            {
                return res;
            }
        }

        static private void WriteFile(string file, List<string> info)
        {
            FileInfo fileinfo = new FileInfo(file);
            FileStream filestream = fileinfo.Create();
            StreamWriter witer = new StreamWriter(filestream);
            foreach (var l in info)
            {
                witer.WriteLine(l);
                witer.Flush();
            }
            witer.Close();
        }

        static public void SaveToFile()
        {
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.OpenSubKey("*\\shell\\PrintRename");
            string url = appkey.GetValue("icon") as string;
            if (string.IsNullOrWhiteSpace(url))
                return;
            string fmturl = Path.GetDirectoryName(url) + "\\format.txt";
            string fnurl = Path.GetDirectoryName(url) + "\\facename.txt";
            WriteFile(fmturl , lstFormat);
            WriteFile(fnurl, lstFaceName);
        }
    }
}
