using System.Collections.Generic;
using System.IO;
using System.Linq;
using Settings;

namespace LicenseSoftware.Reporting
{
    internal sealed class DepartmentReporter : Reporter
    {
        public override void Run(List<string> args)
        {
            ReportTitle = "Отчет по департаментам";
            var licensesIds = args.Aggregate("", (current, str) => current + str + ",");
            licensesIds = licensesIds.TrimEnd(',');
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "department_report.xml")},
                {"connectionString", LicenseSoftwareSettings.ConnectionString},
                {"licensesIds", licensesIds}
            };
            base.Run(arguments);
        }
    }
}
