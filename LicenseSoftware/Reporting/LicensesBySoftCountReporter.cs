using LicenseSoftware.DataModels;
using Registry.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using DataModels.DataModels;
using Settings;

namespace LicenseSoftware.Reporting
{
    internal sealed class LicensesBySoftCountReporter : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Количество лицензий по ПО";
            DataTable departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            string departmentIds = "";
            foreach (DataRow department in departments.Rows)
                if (department.Field<bool>("AllowSelect") == true)
                    departmentIds += department.Field<int>("ID Department").ToString() + ",";
            departmentIds = departmentIds.TrimEnd(',');
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "licenses_by_soft_count.xml"));
            arguments.Add("connectionString", LicenseSoftwareSettings.ConnectionString);
            arguments.Add("departmentIds", departmentIds);
            base.Run(arguments);
        }
    }
}
