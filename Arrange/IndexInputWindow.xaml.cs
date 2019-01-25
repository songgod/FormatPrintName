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
using System.Windows.Shapes;

namespace Arrange
{
    public class IntValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            int d = 0;
            if (int.TryParse(value.ToString(), out d))
            {
                if (d > 0)
                    return new ValidationResult(true, null);
            }

            return new ValidationResult(false, "Validation Failed");
        }
    }

    /// <summary>
    /// IndexInputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class IndexInputWindow : Window
    {
        public IndexInputWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }



        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Index.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(IndexInputWindow), new PropertyMetadata(0));

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
