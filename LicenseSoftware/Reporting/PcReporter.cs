using System.Collections.Generic;
using System.IO;
using LicenseSoftware.DataModels.DataModels;
using Settings;

namespace LicenseSoftware.Reporting
{
    internal sealed class PcReporter : Reporter
    {
        public override void Run(List<string> args)
        {
            ReportTitle = "Отчет для каждого ПК";
            var departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            var installationIds = "";
            foreach (var str in args)
                installationIds += str + ",";
            installationIds = installationIds.TrimEnd(',');
            var arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "pc_report.xml"));
            arguments.Add("connectionString", LicenseSoftwareSettings.ConnectionString);
            //arguments.Add("departmentIds", departmentIds);
            arguments.Add("installationIds", installationIds);
            base.Run(arguments);
        }
    }
}
