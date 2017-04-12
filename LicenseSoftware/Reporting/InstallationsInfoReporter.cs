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
    internal sealed class InstallationsInfoReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Информация по установкам ПО";
            DataTable departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            string departmentIds = "";
            foreach (DataRow department in departments.Rows)
                if (department.Field<bool>("AllowSelect") == true)
                    departmentIds += department.Field<int>("ID Department").ToString() + ",";
            departmentIds = departmentIds.TrimEnd(',');
            Dictionary<string, string> arguments = new Dictionary<string, string>();
            arguments.Add("config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "installations_info.xml"));
            arguments.Add("connectionString", LicenseSoftwareSettings.ConnectionString);
            arguments.Add("departmentIds", departmentIds);
            base.Run(arguments);
        }
    }
}
