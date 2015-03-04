using Registry.Reporting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LicenseSoftware.Reporting
{
    internal sealed class LogLicensesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Лог лицензий на ПО";
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "log_licenses.xml"));
            arguments.Add("connectionString", LicenseSoftwareSettings.ConnectionString);
            using (DateRangeForm drForm = new DateRangeForm())
            {
                if (drForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", drForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", drForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    base.Run(arguments);
                }
                else
                    base.Cancel();
            }
        }
    }
}
