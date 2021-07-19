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
        public string Indexname { get; set; }
        public float Meter { get; set; }
        public int Count { get; set; }
    }

    public delegate void Log(string str);

    public class Processor : DependencyObject
    {
        public static Processor Instance { get; set; }

        public Processor()
        {
            FacePathTuples = new List<Tuple<string, string, List<string>, List<string>, List<float>, List<string>>>();
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

        public int StartIndex { get; set; }

        public float Ratio { get; set; }


        // 面料名称、面料编号、规则名列表、文件路径列表、米数列表、图号列表
        public List<Tuple<string,string, List<string>, List<string>,List<float>,List<string>>> FacePathTuples { get; set; }

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

        private bool ParseFileName(string filename, out FileInfo fi, out string regname)
        {
            fi = new FileInfo();
            regname = filename;
            var strs = filename.Split('-');
            if (strs.Count() != 5 || strs[0]=="小样")
            {
                if(strs.Count()==6)
                {
                    string strtu = strs[0].Substring(0, 1);
                    if (strtu == "图")
                    {
                        int sindex = filename.IndexOf('-');
                        regname = filename.Substring(sindex + 1, filename.Count() - sindex - 1);
                        strs = regname.Split('-');
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            fi.Format = strs[0];
            fi.Disc = strs[1];
            fi.Indexname = strs[2];
            float f = 0.0f;
            if (!float.TryParse(strs[4].Substring(0,strs[4].IndexOf('(')), out f))
            {
                Log(filename + "不符合命名规则");
                return false;
            }
            fi.Meter = f;
            int i = 0;
            if (!int.TryParse(strs[3].Substring(0, strs[3].IndexOf('(')), out i))
            {
                Log(filename + "不符合命名规则");
                return false;
            }
            fi.Count = i;
            return true;
        }

        public event Log OnLog;

        private void Log(string str)
        {
            if (OnLog != null)
                OnLog.Invoke(str);
        }

        public void Process(int startindex, float ratio)
        {
            StartIndex = startindex;
            Ratio = ratio;
            if (!ProcessRename())
                return;
            ProcessExcel();
        }

        private void ProcessExcel()
        {
            Log("导出Excel文件...");
            ExcelEdit ed = new ExcelEdit();
            ed.Create();
            int index = 1;

            ed.SetCellValue("Sheet1", index, 1, "图序号范围");
            ed.SetCellValue("Sheet1", index, 2, "面料编号");
            ed.SetCellValue("Sheet1", index, 3, "面料名称");
            ed.SetCellValue("Sheet1", index, 4, "总米数");
            ed.SetCellValue("Sheet1", index, 5, "实际米数");
            index++;
            // 面料名称、面料编号、规则名列表、文件路径列表、米数列表、图号列表
            foreach (var item in FacePathTuples)
            {
                if (item.Item3.Count == 0)
                    continue;
                string strnumrange = item.Item6.Count >= 2 ? item.Item6.First() + "-" + item.Item6.Last() : item.Item6[0];
                ed.SetCellValue("Sheet1", index, 1, strnumrange);
                double totalmeter = 0.0;
                foreach (var fi in item.Item5)
                {
                    totalmeter += fi;
                }
                ed.SetCellValue("Sheet1", index, 2, item.Item2);
                ed.SetCellValue("Sheet1", index, 3, item.Item1);
                ed.SetCellValue("Sheet1", index, 4, totalmeter);
                ed.SetCellValue("Sheet1", index, 5, item.Item5.Count * Ratio + totalmeter);
                index++;
            }
            string excelname = Url.Substring(Url.LastIndexOf('\\')+1);
            ExcelUrl = Url + "\\" + excelname + ".xlsx";
            ed.SaveAs(ExcelUrl);
            ed.Close();
            Log("导出Excel文件结束");

            if (MessageBox.Show("是否更新库存表?", "更新", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                bool b0 = System.IO.File.Exists(ExcelUrl);
                if(!b0)
                {
                    Log("找不到样表文件"+ExcelUrl+"更新失败");
                    return;
                }
                string stockexcel = Url + "\\库存.xlsx";
                bool b2 = System.IO.File.Exists(stockexcel);
                if (!b2)
                {
                    Log("找不到库存表文件" + stockexcel + "更新失败");
                    return;
                }

                try
                {
                    if (!CreateStock.Create(stockexcel, ExcelUrl, ""))
                    {
                        Log("更新库存表失败！请检查文件内容有效性！");
                    }
                    else
                        Log("更新库存表成功！");
                }
                catch (Exception)
                {

                    Log("更新库存表失败！请检查文件内容有效性！");
                }

            }
        }

        private bool ProcessRename()
        {
            Log("查找文件...");
            List<string> files = new List<string>();
            GetAllFiles(Url, ref files);
            if (files.Count == 0)
            {
                Log("没有找到文件，结束");
                return false;
            }

            Log("查找米样文件...");
            int nCount = Config.Settings.lstFaceInfo.Count;
            for (int i = 0; i < nCount; i++)
            {
                // 面料名称、面料编号、规则名列表、文件路径列表、米数列表、图号列表
                FacePathTuples.Add(new Tuple<string,string, List<string>, List<string>,List<float>, List<string>>(Config.Settings.lstFaceInfo[i].FaceName,Config.Settings.lstFaceInfo[i].Index.ToString()+"#", new List<string>(), new List<string>(),new List<float>(), new List<string>()));
            }

            nCount = 0;
            foreach (var f in files)
            {
                FileInfo fi;string regname;
                if (ParseFileName(Path.GetFileNameWithoutExtension(f), out fi, out regname))
                {
                    var res = FacePathTuples.Where(s => s.Item2 == fi.Indexname);
                    if (res.Count() != 0)
                    {
                        res.ElementAt(0).Item3.Add(regname);
                        res.ElementAt(0).Item4.Add(f);
                        res.ElementAt(0).Item5.Add(fi.Meter);
                        nCount++;
                    }
                    else
                    {
                        Log("文件" + f + "的面料类型没有定义");
                    }
                }
            }

            if (nCount == 0)
            {
                Log("没有找到米样文件，结束");
                return false;
            }

            Log("找到" + nCount.ToString() + "个米样文件，开始重命名...");
            int totalindex = StartIndex;
            foreach (var item in FacePathTuples)
            {
                int index = 0;
                foreach (var f in item.Item4)
                {
                    string extra = totalindex.ToString() + "图";
                    string newname = extra + "-" + item.Item3[index]+Path.GetExtension(f);
                    Rename(f, newname);
                    item.Item6.Add(extra);
                    index++;
                    totalindex++;
                }
            }

            Log("重命名结束...");

            return true;
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
                    Log("重命名 " + Path.GetFileName(oldname) + " 为 "+Path.GetFileName(newname));
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
