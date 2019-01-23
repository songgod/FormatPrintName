using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ArrangeSample
{
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

        
        public void Process()
        {

        }

        public void OpenExcel()
        {

        }
    }
}
