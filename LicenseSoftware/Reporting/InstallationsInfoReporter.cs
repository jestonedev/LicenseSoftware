using System.Collections.Generic;
using System.Data;
using System.IO;
using LicenseSoftware.DataModels.DataModels;
using Settings;

namespace LicenseSoftware.Reporting
{
    internal sealed class InstallationsInfoReporter: Reporter
    {
        public override void Run()
        {
            ReportTitle = "Информация по установкам ПО";
            var departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            var departmentIds = "";
            foreach (DataRow department in departments.Rows)
                if (department.Field<bool>("AllowSelect"))
                    departmentIds += department.Field<int>("ID Department") + ",";
            departmentIds = departmentIds.TrimEnd(',');
            var arguments = new Dictionary<string, string>
            {
                {"config", Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "installations_info.xml")},
                {"connectionString", LicenseSoftwareSettings.ConnectionString},
                {"departmentIds", departmentIds}
            };
            base.Run(arguments);
        }
    }
}
