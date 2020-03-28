using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Collections;

namespace ConfigSetting
{
    [ValueConversion(typeof(IList), typeof(int))]
    public sealed class ItemIndexConverter : FrameworkContentElement, IValueConverter
    {
        public Object Convert(Object data_item, Type t, Object p, CultureInfo _)
        {
            return ((IList)DataContext).IndexOf(data_item);
        }
            

        public Object ConvertBack(Object o, Type t, Object p, CultureInfo _)
        {
            throw new NotImplementedException();
        }
    };

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
        //    try
        //    {
        //        CreateStock.Create(@"E:\WeiYun\GPT\private\doc\米样\软件升级\原软件生成表\库存.xls",
        //@"E:\WeiYun\GPT\private\doc\米样\软件升级\原软件生成表\2-22.xlsx",
        //@"E:\WeiYun\GPT\private\doc\米样\软件升级\原软件生成表\小样2-222.xlsx");
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Config.Settings.SaveToFile();
            this.Close();   
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            Config.Settings.lstFaceInfo.Insert(0, new FaceInfo() { FaceName = "未定义", Temperature = 100, Disc = "无" });
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            int index = lstview.SelectedIndex;
            if (index >= 0 && index < lstview.Items.Count)
            {
                Config.Settings.lstFaceInfo.RemoveAt(index);
            }
        }

        private void up_Click(object sender, RoutedEventArgs e)
        {
            int index = lstview.SelectedIndex;
            if (index < lstview.Items.Count && (index-1)>=0)
            {
                Config.Settings.lstFaceInfo.Move(index, index - 1);
            }
            lstview.Items.Refresh();
        }

        private void down_Click(object sender, RoutedEventArgs e)
        {
            int index = lstview.SelectedIndex;
            if (index >= 0 && (index + 1) <= lstview.Items.Count - 1)
            {
                Config.Settings.lstFaceInfo.Move(index, index + 1);
            }
            lstview.Items.Refresh();
        }
    }
}
