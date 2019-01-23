using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameSample
{
    public class MainApp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            string file = "";
            for (int i = 0; i < args.Count(); i++)
            {
                file += " ";
                file += args[i];
            }
            file = file.Trim();
            if (!File.Exists(file))
                return;
            FileNameInfo.Instance = new FileNameInfo() { OrgFullName = file };
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
