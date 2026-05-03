using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SIIP_Transilvania
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SIIP_Transilvania.Tests.TestSIIP.RunAll();
            Application.Run(new SIIP_Transilvania.Forms.FormMain());
        }
    }
}
