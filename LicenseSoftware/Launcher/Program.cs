using System;
using System.Configuration;
using System.Diagnostics;

namespace Launcher
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var exeApp = ConfigurationManager.AppSettings["exeApp"];
            using (var process = new Process())
            {
                var psi = new ProcessStartInfo(exeApp);
                var argumetns = "";
                foreach (var arg in args)
                {
                    argumetns += arg + " ";
                }
                psi.Arguments = argumetns;
                process.StartInfo = psi;
                process.Start();
            }
        }
    }
}
