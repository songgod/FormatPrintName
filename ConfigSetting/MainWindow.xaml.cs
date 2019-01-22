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

namespace ConfigSetting
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

        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void add_Click(object sender, RoutedEventArgs e)
        {

        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstview_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listview = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Collections.IList list = listview.SelectedItems as System.Collections.IList;
                DataObject data = new DataObject(typeof(System.Collections.IList), list);
                if (list.Count > 0)
                {
                    DragDrop.DoDragDrop(listview, data, DragDropEffects.Move);
                }
            }
        }

        private void lstview_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(System.Collections.IList)))
            {
                System.Collections.IList peopleList = e.Data.GetData(typeof(System.Collections.IList)) as System.Collections.IList;
                //index为放置时鼠标下元素项的索引
                int index = GetCurrentIndex(new GetPositionDelegate(e.GetPosition));
                if (index > -1)
                {
                    FaceInfo Logmess = peopleList[0] as FaceInfo;
                    //拖动元素集合的第一个元素索引
                    int OldFirstIndex = Config.Settings.lstFaceInfo.IndexOf(Logmess);
                    //下边那个循环要求数据源必须为ObservableCollection<T>类型，T为对象
                    for (int i = 0; i < peopleList.Count; i++)
                    {
                        Config.Settings.lstFaceInfo.Move(OldFirstIndex, index);
                    }
                    lstview.SelectedItems.Clear();
                }
            }
        }

        private int GetCurrentIndex(GetPositionDelegate getPosition)
        {
            int index = -1;
            for (int i = 0; i < lstview.Items.Count; ++i)
            {
                ListViewItem item = GetListViewItem(i);
                if (item != null && this.IsMouseOverTarget(item, getPosition))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private bool IsMouseOverTarget(Visual target, GetPositionDelegate getPosition)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = getPosition((IInputElement)target);
            return bounds.Contains(mousePos);
        }

        delegate Point GetPositionDelegate(IInputElement element);

        ListViewItem GetListViewItem(int index)
        {
            if (lstview.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return lstview.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;

        }
    }
}
