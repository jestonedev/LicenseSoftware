using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Windows.Forms;
using LicenseSoftware.Entities;
using System.Data.SqlClient;
using System.Threading;
using System.Globalization;

namespace LicenseSoftware.DataModels
{
    public sealed class DepartmentsDataModel : DataModel
    {
        private static DepartmentsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM Departments";
        private static string accessQuery = @"SELECT ISNULL(d.[ID Department],0) AS [ID Department], d1.[ID Parent Department]
                                                FROM
                                                  dbo.ACLUsers u
                                                  LEFT JOIN dbo.ACLDepartments d
                                                    ON d.[ID User] = u.[ID User]
                                                  INNER JOIN dbo.Departments d1 ON d1.[ID Department] = d.[ID Department]
                                                WHERE
                                                  UserName = suser_sname()";
        private static string tableName = "Departments";

        private DepartmentsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID Department"] };
        }

        public static DepartmentsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public DataTable SelectVisibleDepartments()
        {
            DataTable departments = base.Select();
            DataTable accessDepartments = null;
            DataTable resultDepartments = new DataTable("Departments");
            resultDepartments.Columns.Add("ID Department").DataType = typeof(int);
            resultDepartments.Columns.Add("ID Parent Department").DataType = typeof(int);
            resultDepartments.Columns.Add("Department").DataType = typeof(string);
            resultDepartments.Columns.Add("AllowSelect").DataType = typeof(bool);
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = accessQuery;
                accessDepartments = connection.SqlSelectTable("accessDepartments", command);
            }

            IEnumerable<int> allowDepartmentsIDs = new List<int>();
            IEnumerable<int> parentDepartmentsIDs = new List<int>();
            foreach (DataRow accessDepartment in accessDepartments.Rows)
            {
                allowDepartmentsIDs = allowDepartmentsIDs.Union(new List<int>() { (int)accessDepartment["ID Department"] });
                if (accessDepartment["ID Parent Department"] != DBNull.Value)
                {
                    bool organizationRoot = false;
                    DataRow currentDepartment = accessDepartment;
                    while (!organizationRoot)
                    {
                        currentDepartment = departments.Rows.Find(currentDepartment["ID Parent Department"]);
                        if (currentDepartment != null)
                        {
                            parentDepartmentsIDs = parentDepartmentsIDs.Union(new List<int>() { (int)currentDepartment["ID Department"] });
                            if (currentDepartment["ID Parent Department"] == DBNull.Value)
                                organizationRoot = true;
                        }
                        else
                            organizationRoot = true;
                    }
                }
                allowDepartmentsIDs = allowDepartmentsIDs.Union(DataModelHelper.GetDepartmentSubUnits((int)accessDepartment["ID Department"]));
            }
            foreach (DataRow row in departments.Rows)
                if (allowDepartmentsIDs.Contains((int)row["ID Department"]))
                    resultDepartments.Rows.Add(new object[] {
                        row["ID Department"],
                        row["ID Parent Department"],
                        row["Department"],
                        true
                    });
                else
                    if (parentDepartmentsIDs.Contains((int)row["ID Department"]))
                        resultDepartments.Rows.Add(new object[] {
                        row["ID Department"],
                        row["ID Parent Department"],
                        row["Department"],
                        false
                    });
            return resultDepartments;
        }

        public static DepartmentsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new DepartmentsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
