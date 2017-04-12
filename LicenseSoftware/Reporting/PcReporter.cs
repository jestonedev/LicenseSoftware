using LicenseSoftware.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DataModels.DataModels;

namespace LicenseSoftware.Reporting
{
    internal sealed class PcReporter : Reporter
    {
        public override void Run(List<string> args)
        {
            ReportTitle = "Отчет для каждого ПК";
            DataTable departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            string installationIds = "";
            foreach (string str in args)
                installationIds += str + ",";
            installationIds = installationIds.TrimEnd(',');
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "pc_report.xml"));
            arguments.Add("connectionString", LicenseSoftwareSettings.ConnectionString);
            //arguments.Add("departmentIds", departmentIds);
            arguments.Add("installationIds", installationIds);
            base.Run(arguments);
        }
    }
}
