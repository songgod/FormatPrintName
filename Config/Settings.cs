using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Config
{
    public class FaceInfo : DependencyObject
    {


        public string FaceName
        {
            get { return (string)GetValue(FaceNameProperty); }
            set { SetValue(FaceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FaceName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FaceNameProperty =
            DependencyProperty.Register("FaceName", typeof(string), typeof(FaceInfo), new PropertyMetadata(""));



        public float Temperature
        {
            get { return (float)GetValue(TemperatureProperty); }
            set { SetValue(TemperatureProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Temperature.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TemperatureProperty =
            DependencyProperty.Register("Temperature", typeof(float), typeof(FaceInfo), new PropertyMetadata(0.0f));



        public string Disc
        {
            get { return (string)GetValue(DiscProperty); }
            set { SetValue(DiscProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Disc.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DiscProperty =
            DependencyProperty.Register("Disc", typeof(string), typeof(FaceInfo), new PropertyMetadata(""));


    }
    static public class Settings
    {
        static Settings()
        {
            lstFormat = new ObservableCollection<string>();
            lstFaceInfo = new ObservableCollection<FaceInfo>();
            lstFaceName = new ObservableCollection<string>();
            LoadFromFile();
        }

        static public ObservableCollection<string> lstFormat { get; set; }

        static public ObservableCollection<FaceInfo> lstFaceInfo { get; set; }

        static public ObservableCollection<string> lstFaceName { get; set; }

        static public void LoadFromFile()
        {
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            RegistryKey appkey = rootKey.OpenSubKey("*\\shell\\PrintRename");
            string url = appkey.GetValue("icon") as string;
            if (string.IsNullOrWhiteSpace(url))
                return;
            string fmturl = Path.GetDirectoryName(url) + "\\format.txt";
            string fnurl = Path.GetDirectoryName(url) + "\\facename.txt";
            ReadFormatFile(fmturl);
            ReadFaceFile(fnurl);
        }

        static private void ReadFormatFile(string file)
        {
            lstFormat = new ObservableCollection<string>();
            try
            {
                StreamReader reader = new StreamReader(file, Encoding.Default);
                if (reader == null)
                    return;

                string l = reader.ReadLine();
                while (!string.IsNullOrWhiteSpace(l))
                {
                    lstFormat.Add(l);
                    l = reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception)
            {
                return;
            }
        }

        static private void ReadFaceFile(string file)
        {
            lstFaceInfo = new ObservableCollection<FaceInfo>();
            lstFaceName = new ObservableCollection<string>();
            try
            {
                StreamReader reader = new StreamReader(file, Encoding.Default);
                if (reader == null)
                    return;

                string l = reader.ReadLine();
                while (!string.IsNullOrWhiteSpace(l))
                {
                    string[] strs = l.Split(' ');
                    if(strs.Count()==3)
                    {
                        float d = 0.0f;
                        if(float.TryParse(strs[1],out d))
                        {
                            lstFaceName.Add(strs[0]);
                            lstFaceInfo.Add(new FaceInfo() { FaceName = strs[0], Temperature=d, Disc = strs[2]});
                        }
                    }
                    l = reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception)
            {
                return;
            }
        }

        static private void WriteFormatFile(string file, ObservableCollection<string> info)
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

        static private void WriteFaceFile(string file, ObservableCollection<FaceInfo> info)
        {
            FileInfo fileinfo = new FileInfo(file);
            FileStream filestream = fileinfo.Create();
            StreamWriter witer = new StreamWriter(filestream);
            foreach (var l in info)
            {
                string s = l.FaceName + " " + l.Temperature + " " + l.Disc;
                witer.WriteLine(s);
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
            WriteFormatFile(fmturl , lstFormat);
            WriteFaceFile(fnurl, lstFaceInfo);
        }

        static public void SortFaceInfoByTemperature()
        {
            List<FaceInfo> res = new List<FaceInfo>(lstFaceInfo.OrderBy(s => s.Temperature));
            lstFaceInfo.Clear();
            foreach (var item in res)
            {
                lstFaceInfo.Add(item);
            }
        }
    }
}
