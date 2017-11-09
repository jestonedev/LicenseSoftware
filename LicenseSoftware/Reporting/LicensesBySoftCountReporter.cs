using System.Collections.Generic;
using System.Data;
using System.IO;
using LicenseSoftware.DataModels.DataModels;
using Settings;

namespace LicenseSoftware.Reporting
{
    internal sealed class LicensesBySoftCountReporter : Reporter
    {
        public override void Run()
        {
            ReportTitle = "Количество лицензий по ПО";
            var departments = DepartmentsDataModel.GetInstance().SelectVisibleDepartments();
            var departmentIds = "";
            foreach (DataRow department in departments.Rows)
                if (department.Field<bool>("AllowSelect"))
                    departmentIds += department.Field<int>("ID Department") + ",";
            departmentIds = departmentIds.TrimEnd(',');
            var arguments = new Dictionary<string, string>
            {
                {
                    "config",
                    Path.Combine(LicenseSoftwareSettings.ActivityManagerConfigsPath, "licenses_by_soft_count.xml")
                },
                {"connectionString", LicenseSoftwareSettings.ConnectionString},
                {"departmentIds", departmentIds}
            };
            base.Run(arguments);
        }
    }
}
