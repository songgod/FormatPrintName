using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using Config;

namespace Arrange
{
    public struct FileInfo
    {
        public string Format { get; set; }
        public string Disc { get; set; }
        public string Facename { get; set; }
        public float Meter { get; set; }
        public int Count { get; set; }
    }

    public class Processor : DependencyObject
    {
        public static Processor Instance { get; set; }

        public Processor()
        {
            Logs = new ObservableCollection<string>();
            FacePathTuples = new List<Tuple<string, List<string>, List<float>, List<string>>>();
        }

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(Processor), new PropertyMetadata(""));

        public string ExcelUrl { get; set; }

        public ObservableCollection<string> Logs
        {
            get { return (ObservableCollection<string>)GetValue(LogsProperty); }
            set { SetValue(LogsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Logs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogsProperty =
            DependencyProperty.Register("Logs", typeof(ObservableCollection<string>), typeof(Processor), new PropertyMetadata(null));

        public List<Tuple<string,List<string>,List<float>,List<string>>> FacePathTuples { get; set; }

        private void GetAllFiles(string dir, ref List<string> files)
        {
            var rfiles = Directory.GetFiles(dir);
            foreach (var file in rfiles)
            {
                files.Add(file);
            }
            var dirs = Directory.GetDirectories(dir);
            foreach (var tdir in dirs)
            {
                GetAllFiles(tdir, ref files);
            }
        }

        private bool ParseFileName(string filename, out FileInfo fi)
        {
            fi = new FileInfo();
            var strs = filename.Split('-');
            if (strs.Count() != 5)
            {
                return false;
            }
            fi.Format = strs[0];
            fi.Disc = strs[1];
            fi.Facename = strs[2];
            float f = 0.0f;
            if (!float.TryParse(strs[3].Substring(0,strs[3].IndexOf('(')), out f))
                return false;
            fi.Meter = f;
            int i = 0;
            if (!int.TryParse(strs[4].Substring(0, strs[4].IndexOf('(')), out i))
                return false;
            fi.Count = i;
            return true;
        }
        

        public void Process()
        {
            if (!ProcessRename())
                return;
            ProcessExcel();
        }

        private void ProcessExcel()
        {
            Logs.Add("导出Excel文件...");
            ExcelEdit ed = new ExcelEdit();
            ed.Create();
            int index = 1;

            ed.SetCellValue("Sheet1", index, 1, "面料名称");
            ed.SetCellValue("Sheet1", index, 2, "图序号范围");
            ed.SetCellValue("Sheet1", index, 3, "总米数");
            index++;
            foreach (var item in FacePathTuples)
            {
                if (item.Item2.Count == 0)
                    continue;
                ed.SetCellValue("Sheet1", index, 1, item.Item1);
                string strnumrange = item.Item4.Count>=2 ? item.Item4.First()+"-"+item.Item4.Last() : item.Item4[0];
                ed.SetCellValue("Sheet1", index, 2, strnumrange);
                double totalmeter = 0.0;
                foreach (var fi in item.Item3)
                {
                    totalmeter += fi;
                }
                ed.SetCellValue("Sheet1", index, 3, totalmeter);
                index++;
            }
            string excelname = Url.Substring(Url.LastIndexOf('\\')+1);
            ExcelUrl = Url + "\\" + excelname + ".xlsx";
            ed.SaveAs(ExcelUrl);
            ed.Close();
            Logs.Add("导出Excel文件结束");
        }

        private bool ProcessRename()
        {
            Logs.Add("查找文件...");
            List<string> files = new List<string>();
            GetAllFiles(Url, ref files);
            if (files.Count == 0)
            {
                Logs.Add("没有找到文件，结束");
                return false;
            }

            Logs.Add("查找米样文件...");
            int nCount = Config.Settings.lstFaceInfo.Count;
            for (int i = 0; i < nCount; i++)
            {
                FacePathTuples.Add(new Tuple<string, List<string>,List<float>, List<string>>(Config.Settings.lstFaceInfo[i].FaceName, new List<string>(),new List<float>(), new List<string>()));
            }

            nCount = 0;
            foreach (var f in files)
            {
                FileInfo fi;
                if (ParseFileName(Path.GetFileNameWithoutExtension(f), out fi))
                {
                    var res = FacePathTuples.Where(s => s.Item1 == fi.Facename);
                    if (res.Count() != 0)
                    {
                        res.ElementAt(0).Item2.Add(f);
                        res.ElementAt(0).Item3.Add(fi.Meter);
                        nCount++;
                    }
                    else
                    {
                        Logs.Add("文件" + f + "的面料类型没有定义");
                    }
                }
                else
                {
                    Logs.Add("文件" + f + "不符合米样命名规则");
                }
            }

            if (nCount == 0)
            {
                Logs.Add("没有找到米样文件，结束");
                return false;
            }

            Logs.Add("找到" + nCount.ToString() + "个米样文件，开始重命名...");
            int totalindex = 1;
            foreach (var item in FacePathTuples)
            {
                int index = 0;
                foreach (var f in item.Item2)
                {
                    string extra = "图" + totalindex.ToString();
                    string newname = extra + "-" + Path.GetFileName(f);
                    Rename(f, newname);
                    //item.Item2[index] = Path.GetDirectoryName(f) + "\\" + newname;
                    item.Item4.Add(extra);
                    index++;
                    totalindex++;
                }
            }

            Logs.Add("重命名结束...");

            return true;
        }

        private bool Rename(string oldname, string newname)
        {

            Computer my = new Computer();

            while (true)
            {
                try
                {
                    my.FileSystem.RenameFile(oldname, newname);
                    Logs.Add("重名名 " + Path.GetFileName(oldname) + " 为 "+Path.GetFileName(newname));
                    break;
                }
                catch (Exception e)
                {
                    MessageBox.Show("重命名" + oldname + "失败，原因:\n" + e.Message+"\n请尝试修复原因后点击确定重试", "提示", MessageBoxButton.OK);
                    continue;
                }
            }

            return true;
        }

        public void OpenExcel()
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(ExcelUrl);
            if (fi.Exists)
            {
                System.Diagnostics.Process.Start(ExcelUrl);
            }
        }
    }
}
