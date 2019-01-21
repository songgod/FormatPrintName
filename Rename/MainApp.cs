using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rename
{
    public class MainApp
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Count() != 1)
                return;
            FileNameInfo.Instance = new FileNameInfo() { OrgFullName = args[0] };
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
