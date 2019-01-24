using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RenameSample
{
    public class FaceNameCount : DependencyObject
    {
        private static void OnFaceNameCountChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
        {
            if(FileNameInfo.Instance!=null)
                FileNameInfo.Instance.UpdateNewName();
        }

        public string FaceName
        {
            get { return (string)GetValue(FaceNameProperty); }
            set { SetValue(FaceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FaceName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FaceNameProperty =
            DependencyProperty.Register("FaceName", typeof(string), typeof(FaceNameCount), new PropertyMetadata("", new PropertyChangedCallback(OnFaceNameCountChangedCallback)));



        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(FaceNameCount), new PropertyMetadata(1, new PropertyChangedCallback(OnFaceNameCountChangedCallback)));


    }

    public class FaceNameCounts : ObservableCollection<FaceNameCount>
    {

    }

    public class FileNameInfo : DependencyObject
    {
        public static FileNameInfo Instance { get; set; }

        public FileNameInfo()
        {
            FaceNameCounts = new FaceNameCounts();
            FaceNameCounts.CollectionChanged += FaceNameCounts_CollectionChanged;
        }

        public string OrgFullName
        {
            get { return (string)GetValue(OrgFullNameProperty); }
            set { SetValue(OrgFullNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrgFullName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrgFullNameProperty =
            DependencyProperty.Register("OrgFullName", typeof(string), typeof(FileNameInfo), new PropertyMetadata("", new PropertyChangedCallback(OnOrgFullNameChangedCallback)));

        private static void OnOrgFullNameChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
        {
            FileNameInfo owner = (FileNameInfo)obj;
            
            owner.OrgName = Path.GetFileName(owner.OrgFullName);
            var strs = owner.OrgName.Split('-');
            if(strs.Count()>=3 && strs[0]=="小样")
            {
                owner.Format = strs[1];
                owner.Disc = strs[2];
                for (int i = 3; i < strs.Count(); i++)
                {
                    string fnc = strs[i];
                    int sindex = fnc.IndexOf('(');
                    int eindex = fnc.IndexOf(')');
                    if(sindex>0 && eindex >0 && sindex<eindex)
                    {
                        string sfn = fnc.Substring(0, sindex);
                        string sc = fnc.Substring(sindex + 1, eindex - sindex - 1);
                        int ncount = 0;
                        if(int.TryParse(sc,out ncount) && ncount>0)
                            owner.FaceNameCounts.Add(new FaceNameCount() { FaceName = sfn, Count = ncount });
                    }
                }
                owner.UpdateNewName();
            }
            else
            {
                owner.Format = Config.Settings.lstFormat.Count == 0 ? "" : Config.Settings.lstFormat[0];
            }
        }


        public string OrgName
        {
            get { return (string)GetValue(OrgNameProperty); }
            set { SetValue(OrgNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for OrgName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrgNameProperty =
            DependencyProperty.Register("OrgName", typeof(string), typeof(FileNameInfo), new PropertyMetadata(""));


        private static void OnRefInfoChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
        {
            FileNameInfo owner = (FileNameInfo)obj;
            owner.UpdateNewName();
        }

        private void FaceNameCounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateNewName();
        }

        public void UpdateNewName()
        {
            string name = "小样-";
            name += Format;
            name += "-";
            name += Disc;
            name += "-";
            foreach (var item in FaceNameCounts)
            {
                name += item.FaceName;
                name += "(";
                name += item.Count.ToString();
                name += ")个";
                if (FaceNameCounts.Last() != item)
                    name += "-";
            }
            name += Path.GetExtension(OrgFullName);
            NewName = name;
        }

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Format.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register("Format", typeof(string), typeof(FileNameInfo), new PropertyMetadata("", new PropertyChangedCallback(OnRefInfoChangedCallback)));



        public string Disc
        {
            get { return (string)GetValue(DiscProperty); }
            set { SetValue(DiscProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Disc.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DiscProperty =
            DependencyProperty.Register("Disc", typeof(string), typeof(FileNameInfo), new PropertyMetadata("无", new PropertyChangedCallback(OnRefInfoChangedCallback)));




        public FaceNameCounts FaceNameCounts
        {
            get { return (FaceNameCounts)GetValue(FaceNameCountsProperty); }
            set { SetValue(FaceNameCountsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FaceNameCounts.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FaceNameCountsProperty =
            DependencyProperty.Register("FaceNameCounts", typeof(FaceNameCounts), typeof(FileNameInfo), new PropertyMetadata(null, new PropertyChangedCallback(OnRefInfoChangedCallback)));



        public string NewName
        {
            get { return (string)GetValue(NewNameProperty); }
            set { SetValue(NewNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NewName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NewNameProperty =
            DependencyProperty.Register("NewName", typeof(string), typeof(FileNameInfo), new PropertyMetadata(""));

        public bool Rename()
        {
            string newpath = Path.GetDirectoryName(OrgFullName)+"\\" + NewName;
            if(File.Exists(newpath))
            {
                MessageBox.Show("存在同名文件！");
                return false;
            }
            Computer my = new Computer();

            try
            {
                my.FileSystem.RenameFile(OrgFullName, NewName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }


    }
}
