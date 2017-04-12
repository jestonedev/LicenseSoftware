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
    internal sealed class DepartmentReporter : Reporter
    {
        public override void Run(List<string> args)
        {
            ReportTitle = "Отчет по департаментам";
            DataTable departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            //DataTable licenses;
            string licensesIds = "";
            foreach (string str in args)
                licensesIds += str + ",";
            licensesIds = licensesIds.TrimEnd(',');
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            //arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "installations_info.xml"));
            arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "department_report.xml"));
            arguments.Add("connectionString", LicenseSoftwareSettings.ConnectionString);
            //arguments.Add("departmentIds", departmentIds);
            arguments.Add("licensesIds", licensesIds);
            base.Run(arguments);
        }
    }
}
