using System;
using System.Configuration;
using System.Diagnostics;

namespace Launcher
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var exeApp = ConfigurationManager.AppSettings["exeApp"];
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo(exeApp);
                process.StartInfo = psi;
                process.Start();
            }
        }
    }
}
