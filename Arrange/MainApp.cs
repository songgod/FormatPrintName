using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrange
{
    public class MainApp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string dir = "";
            for (int i = 0; i < args.Count(); i++)
            {
                dir += args[i];
                dir += " ";
            }
            dir = dir.TrimEnd();
            if (!Directory.Exists(dir))
                return;
            Processor.Instance = new Processor() { Url = dir };
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
