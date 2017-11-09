using Registry.Reporting;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Settings;

namespace LicenseSoftware.Reporting
{
    internal sealed class LogLicensesReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Лог лицензий на ПО";
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "log_licenses.xml")},
                {"connectionString", LicenseSoftwareSettings.ConnectionString}
            };
            using (var drForm = new DateRangeForm())
            {
                if (drForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arguments.Add("date_from", drForm.DateFrom.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    arguments.Add("date_to", drForm.DateTo.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
                    base.Run(arguments);
                }
                else
                    Cancel();
            }
        }
    }
}
