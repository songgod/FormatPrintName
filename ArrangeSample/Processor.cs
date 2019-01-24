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
        public string ReguName { get; set; }
        public string Number { get; set; }
        public string Format { get; set; }
        public string Disc { get; set; }
        public List<Tuple<Tuple<string,string>,int>> FaceNameCount { get; set; }
    }


    public delegate void Log(string str);

    public class Processor : DependencyObject
    {
        public static Processor Instance { get; set; }

        public Processor()
        {
        }

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Url.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UrlProperty =
            DependencyProperty.Register("Url", typeof(string), typeof(Processor), new PropertyMetadata(""));


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

        public event Log OnLog;

        private void Log(string str)
        {
            if (OnLog != null)
                OnLog.Invoke(str);
        }

        private bool ParseFileName(string filename, out FileInfo fi)
        {
            fi = new FileInfo();
            var strs = filename.Split('-');
            fi.ReguName = filename;
            if (strs.Count() < 4)
            {
                return false;
            }

            if (strs[0] != "小样")
            {
                if(strs[0].Last()=='号')
                {
                    int sindex = filename.IndexOf('-') + 1;
                    fi.ReguName = filename.Substring(sindex, filename.Count() - sindex);
                    strs = fi.ReguName.Split('-');
                }
                else
                {
                    return false;
                }
            }
            
            fi.Format = strs[1];
            fi.Disc = strs[2];
            fi.FaceNameCount = new List<Tuple<Tuple<string,string>, int>>();

            for (int i = 3; i < strs.Count(); i++)
            {
                int findex = strs[i].IndexOf('(');
                string indexname = strs[i].Substring(0,findex);
                int lindex = strs[i].IndexOf(')');
                string strcount = strs[i].Substring(findex + 1, lindex - findex - 1);
                int count = 0;
                if (string.IsNullOrWhiteSpace(indexname) || !int.TryParse(strcount, out count))
                {
                    Log(filename + "不符合命名规则");
                    return false;
                }
                var res = Config.Settings.lstFaceInfo.Where(s => (s.Index.ToString() + "#" == indexname));
                string fname = res.Count() > 0 ? res.ElementAt(0).FaceName : "";
                fi.FaceNameCount.Add(new Tuple<Tuple<string,string>, int>(new Tuple<string,string>(fname,indexname), count));
            }
            
            return true;
        }

        public void Process()
        {
            Log("查找文件...");
            List<string> files = new List<string>();
            GetAllFiles(Url, ref files);
            if (files.Count == 0)
            {
                Log("没有找到文件，结束");
                return;
            }

            Log("查找小样文件...");

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
            }

            if (fileinfos.Count == 0)
            {
                Log("没有找到米样文件，结束");
                return;
            }

            Log("找到" + fileinfos.Count.ToString() + "个小样文件，开始重命名...");
            
            foreach (var item in fileinfos)
            {
                string newname = item.Number + "-" + item.ReguName+Path.GetExtension(item.Url);
                Rename(item.Url, newname);
            }

            Log("重命名结束...");

            Log("导出Excel文件...");
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
                        disc += fn.Item1.Item1 + "(" + fn.Item2.ToString() + ")个、";
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
                List<Tuple<string, string, List<int>>> sortfacenamecount = new List<Tuple<string, string, List<int>>>();
                for (int i = 0; i < nCount; i++)
                {
                    sortfacenamecount.Add(new Tuple<string, string, List<int>>(Config.Settings.lstFaceInfo[i].Index.ToString()+"#", Config.Settings.lstFaceInfo[i].FaceName, new List<int>()));
                }

                foreach (var item in fileinfos)
                {
                    foreach (var fn in item.FaceNameCount)
                    {
                        var res = sortfacenamecount.Where(s => s.Item1 == fn.Item1.Item2);
                        if (res.Count() != 0)
                        {
                            res.ElementAt(0).Item3.Add(fn.Item2);
                        }
                        else
                        {
                            Log("文件" + item.Url + "的面料类型"+fn.Item1+"没有定义");
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
                    if (item.Item3.Count == 0)
                        continue;

                    ed.SetCellValue("Sheet1", index, 1, item.Item2);

                    int totalcount = 0;
                    foreach (var fn in item.Item3)
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

            Log("导出Excel文件结束");
        }

        private bool Rename(string oldname, string newname)
        {
            if (Path.GetFileName(oldname) == newname)
            {
                Log("重命名 " + Path.GetFileName(oldname) + " 为 " + Path.GetFileName(newname));
                return true;
            }
            Computer my = new Computer();

            while (true)
            {
                try
                {
                    my.FileSystem.RenameFile(oldname, newname);
                    Log("重命名 " + Path.GetFileName(oldname) + " 为 " + Path.GetFileName(newname));
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
