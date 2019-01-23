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
using System.Windows.Threading;

namespace ArrangeSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Processor.Instance.OnLog += Instance_OnLog;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Processor.Instance.Process();
        }

        private void Instance_OnLog(string str)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new LoadLogDelegate(LoadLog), str);
        }

        private void LoadLog(string log)
        {
            lstbox.Items.Add(log);
        }

        private delegate void LoadLogDelegate(string str);

        private void OpenExcel_Click(object sender, RoutedEventArgs e)
        {
            Processor.Instance.OpenExcel();
        }
    }
}
