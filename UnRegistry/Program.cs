using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace UnRegistry
{
    class Program
    {
        static void Main(string[] args)
        {
            RegistryKey rootKey = Microsoft.Win32.Registry.ClassesRoot;
            rootKey.DeleteSubKeyTree("*\\shell\\PrintRename",false);
            rootKey.DeleteSubKeyTree("*\\shell\\PrintRenameSample", false);
            rootKey.DeleteSubKeyTree("Directory\\shell\\ArrangePrintSample", false);
            rootKey.Close();
        }
    }
}
