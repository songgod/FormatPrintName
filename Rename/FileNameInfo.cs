using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Rename
{
    public class FileNameInfo : DependencyObject
    {
        public static FileNameInfo Instance { get; set; }



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
            string fullname = owner.OrgFullName;
            owner.OrgName = Path.GetFileName(fullname);
            owner.Format = Config.Settings.lstFormat.Count==0 ? "" : Config.Settings.lstFormat[0];
            owner.FaceName = Config.Settings.lstFaceName.Count == 0 ? "" : Config.Settings.lstFaceName[0];
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
            owner.NewName = owner.Format + "-" + owner.Disc + "-" + owner.FaceName + "-" + owner.Meter + "(米)-" + owner.Count + "(个)" + Path.GetExtension(owner.OrgFullName);
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



        public string FaceName
        {
            get { return (string)GetValue(FaceNameProperty); }
            set { SetValue(FaceNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FaceName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FaceNameProperty =
            DependencyProperty.Register("FaceName", typeof(string), typeof(FileNameInfo), new PropertyMetadata("", new PropertyChangedCallback(OnRefInfoChangedCallback)));



        public float Meter
        {
            get { return (float)GetValue(MeterProperty); }
            set { SetValue(MeterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Meter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MeterProperty =
            DependencyProperty.Register("Meter", typeof(float), typeof(FileNameInfo), new PropertyMetadata(6.0f, new PropertyChangedCallback(OnRefInfoChangedCallback)));



        public int Count
        {
            get { return (int)GetValue(CountProperty); }
            set { SetValue(CountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Count.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CountProperty =
            DependencyProperty.Register("Count", typeof(int), typeof(FileNameInfo), new PropertyMetadata(1, new PropertyChangedCallback(OnRefInfoChangedCallback)));



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
