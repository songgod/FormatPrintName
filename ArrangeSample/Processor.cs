using Config;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArrangeSample
{
    struct FileInfo
    {
        public string Url { get; set; }
        public string Number { get; set; }
        public string Format { get; set; }
        public string Disc { get; set; }
        public List<Tuple<string,int>> FaceNameCount { get; set; }
    }

    public class Processor : DependencyObject
    {
        public static Processor Instance { get; set; }

        public Processor()
        {
            Logs = new ObservableCollection<string>();
        }

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(Processor), new PropertyMetadata(""));



        public ObservableCollection<string> Logs
        {
            get { return (ObservableCollection<string>)GetValue(LogsProperty); }
            set { SetValue(LogsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Logs.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LogsProperty =
            DependencyProperty.Register("Logs", typeof(ObservableCollection<string>), typeof(Processor), new PropertyMetadata(null));

        public string ExcelUrl1 { get; set; }
        public string ExcelUrl2 { get; set; }

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
            var strs = filename.Split('-');
            if (strs.Count() < 4 || strs[0] != "小样")
            {
                fi = new FileInfo();
                return false;
            }

            fi = new FileInfo();
            fi.Format = strs[1];
            fi.Disc = strs[2];
            fi.FaceNameCount = new List<Tuple<string, int>>();

            for (int i = 3; i < strs.Count(); i++)
            {
                int findex = strs[i].IndexOf('(');
                string facename = strs[i].Substring(0,findex);
                int lindex = strs[i].IndexOf(')');
                string strcount = strs[i].Substring(findex + 1, lindex - findex - 1);
                int count = 0;
                if (!int.TryParse(strcount, out count))
                    return false;
                fi.FaceNameCount.Add(new Tuple<string, int>(facename, count));
            }
            
            return true;
        }

        public void Process()
        {
            Logs.Add("查找文件...");
            List<string> files = new List<string>();
            GetAllFiles(Url, ref files);
            if (files.Count == 0)
            {
                Logs.Add("没有找到文件，结束");
                return;
            }

            Logs.Add("查找小样文件...");

            List<FileInfo> fileinfos = new List<FileInfo>();
            int totalindex = 1;
            foreach (var f in files)
            {
                FileInfo fi;
                if (ParseFileName(Path.GetFileNameWithoutExtension(f), out fi))
                {
                    fi.Url = f;
                    fi.Number = totalindex.ToString() + "号";
                    fileinfos.Add(fi);
                    totalindex++;
                }
                else
                {
                    Logs.Add("文件" + f + "不符合小样命名规则");
                }
            }

            if (fileinfos.Count == 0)
            {
                Logs.Add("没有找到米样文件，结束");
                return;
            }

            Logs.Add("找到" + fileinfos.Count.ToString() + "个小样文件，开始重命名...");
            
            foreach (var item in fileinfos)
            {
                string newname = item.Number + "-" + Path.GetFileName(item.Url);
                Rename(item.Url, newname);
            }

            Logs.Add("重命名结束...");

            Logs.Add("导出Excel文件...");
            {
                ExcelEdit ed = new ExcelEdit();
                ed.Create();
                int index = 1;

                ed.SetCellValue("Sheet1", index, 1, "图号");
                ed.SetCellValue("Sheet1", index, 2, "描述");
                index++;
                foreach (var item in fileinfos)
                {
                    ed.SetCellValue("Sheet1", index, 1, item.Number);

                    string disc = item.Format+item.Disc+"-";
                    foreach (var fn in item.FaceNameCount)
                    {
                        disc += fn.Item1 + "(" + fn.Item2.ToString() + ")个、";
                    }
                    disc.TrimEnd('、');

                    ed.SetCellValue("Sheet1", index, 2, disc);

                    index++;
                }
                string excelname = Url.Substring(Url.LastIndexOf('\\') + 1);
                ExcelUrl1 = Url + "\\小样" + excelname + "1.xlsx";
                ed.SaveAs(ExcelUrl1);
                ed.Close();
            }

            {
                int nCount = Config.Settings.lstFaceInfo.Count;
                List<Tuple<string, List<int>>> sortfacenamecount = new List<Tuple<string, List<int>>>();
                for (int i = 0; i < nCount; i++)
                {
                    sortfacenamecount.Add(new Tuple<string, List<int>>(Config.Settings.lstFaceInfo[i].FaceName, new List<int>()));
                }

                foreach (var item in fileinfos)
                {
                    foreach (var fn in item.FaceNameCount)
                    {
                        var res = sortfacenamecount.Where(s => s.Item1 == fn.Item1);
                        if (res.Count() != 0)
                        {
                            res.ElementAt(0).Item2.Add(fn.Item2);
                        }
                        else
                        {
                            Logs.Add("文件" + item.Url + "的面料类型"+fn.Item1+"没有定义");
                        }
                    }
                }

                ExcelEdit ed = new ExcelEdit();
                ed.Create();
                int index = 1;

                ed.SetCellValue("Sheet1", index, 1, "面料名称");
                ed.SetCellValue("Sheet1", index, 2, "个数");
                index++;



                foreach (var item in sortfacenamecount)
                {
                    ed.SetCellValue("Sheet1", index, 1, item.Item1);

                    int totalcount = 0;
                    foreach (var fn in item.Item2)
                    {
                        totalcount += fn;
                    }

                    ed.SetCellValue("Sheet1", index, 2, totalcount);

                    index++;
                }
                string excelname = Url.Substring(Url.LastIndexOf('\\') + 1);
                ExcelUrl2 = Url + "\\小样" + excelname + "2.xlsx";
                ed.SaveAs(ExcelUrl2);
                ed.Close();
            }

            Logs.Add("导出Excel文件结束");
        }

        private bool Rename(string oldname, string newname)
        {

            Computer my = new Computer();

            while (true)
            {
                try
                {
                    my.FileSystem.RenameFile(oldname, newname);
                    Logs.Add("重名名 " + Path.GetFileName(oldname) + " 为 " + Path.GetFileName(newname));
                    break;
                }
                catch (Exception e)
                {
                    MessageBox.Show("重命名" + oldname + "失败，原因:\n" + e.Message + "\n请尝试修复原因后点击确定重试", "提示", MessageBoxButton.OK);
                    continue;
                }
            }

            return true;
        }

        public void OpenExcel()
        {
            System.IO.FileInfo fi1 = new System.IO.FileInfo(ExcelUrl1);
            if (fi1.Exists)
            {
                System.Diagnostics.Process.Start(ExcelUrl1);
            }
            System.IO.FileInfo fi2 = new System.IO.FileInfo(ExcelUrl2);
            if (fi2.Exists)
            {
                System.Diagnostics.Process.Start(ExcelUrl2);
            }
        }
    }
}
