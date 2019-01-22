using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RenameSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool b = FileNameInfo.Instance.Rename();
            if (b)
                this.Close();
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void add_Click(object sender, RoutedEventArgs e)
        {
            FileNameInfo.Instance.FaceNameCounts.Add(
                new FaceNameCount()
                {
                    FaceName = Config.Settings.lstFaceName.Count == 0 ? "" : Config.Settings.lstFaceName[0],
                    Count = 1
                }
            );
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            int selectindex = lstview.SelectedIndex;
            if (selectindex < 0 || selectindex >= lstview.Items.Count)
                return;
            FileNameInfo.Instance.FaceNameCounts.RemoveAt(selectindex);
        }
    }
}
