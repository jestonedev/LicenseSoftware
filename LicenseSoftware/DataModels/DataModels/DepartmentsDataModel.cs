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
        private static string selectQuery = "SELECT * FROM Departments ORDER BY [ID Parent Department], Department";
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
            resultDepartments.Columns.Add("Level").DataType = typeof(int);
            resultDepartments.PrimaryKey = new DataColumn[] { resultDepartments.Columns["ID Department"] };
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
                        true,
                        0
                    });
                else
                    if (parentDepartmentsIDs.Contains((int)row["ID Department"]))
                        resultDepartments.Rows.Add(new object[] {
                        row["ID Department"],
                        row["ID Parent Department"],
                        row["Department"],
                        false,
                        0
                    });
            return TabulateResultDepartments(SortResultDepartments(resultDepartments));
        }

        private DataTable TabulateResultDepartments(DataTable departments)
        {
            foreach (DataRow department in departments.Rows)
            {
                DataRow currentRow = department;
                int level = 0;
                while (true)
                {
                    if (currentRow["ID Parent Department"] == DBNull.Value)
                        break;
                    currentRow = departments.Rows.Find(currentRow["ID Parent Department"]);
                    level += 1;
                }
                department["Level"] = level;
            }
            foreach (DataRow department in departments.Rows)
            {
                int level = (int)department["Level"];
                for (int i = 0; i < level; i++)
                    department["Department"] = "     "+department["Department"];
            }
            return departments;
        }

        private DataTable SortResultDepartments(DataTable departments)
        {
            DataTable resultDepartments = new DataTable("Departments");
            resultDepartments.Columns.Add("ID Department").DataType = typeof(int);
            resultDepartments.Columns.Add("ID Parent Department").DataType = typeof(int);
            resultDepartments.Columns.Add("Department").DataType = typeof(string);
            resultDepartments.Columns.Add("AllowSelect").DataType = typeof(bool);
            resultDepartments.Columns.Add("Level").DataType = typeof(int);
            resultDepartments.PrimaryKey = new DataColumn[] { resultDepartments.Columns["ID Department"] };
            foreach (DataRow department in departments.Rows)
            {
                int index =  -1;  //По умолчанию вставляем в конец
                int parentIndex = -1;
                for (int i = 0; i < resultDepartments.Rows.Count; i ++ )
                {
                    //Ищем точку, в которую необходимо вставить запись
                    if (department["ID Parent Department"] == DBNull.Value)
                        break;
                    if ((int)department["ID Parent Department"] == (int)resultDepartments.Rows[i]["ID Department"])
                        parentIndex = i;
                    else
                        if (parentIndex != -1 &&
                            (resultDepartments.Rows[i]["ID Parent Department"] == DBNull.Value ||
                            (int)resultDepartments.Rows[i]["ID Parent Department"] != (int)resultDepartments.Rows[parentIndex]["ID Department"]))
                        {
                            index = i;
                            break;
                        }
                }
                DataRow row = resultDepartments.NewRow();
                row["ID Department"] = department["ID Department"];
                row["ID Parent Department"] = department["ID Parent Department"];
                row["Department"] = department["Department"];
                row["AllowSelect"] = department["AllowSelect"];
                row["Level"] = department["Level"];
                if (index == -1 && parentIndex == -1)
                    resultDepartments.Rows.Add(row);
                else
                    if (index == -1 && parentIndex != -1)
                        resultDepartments.Rows.InsertAt(row, parentIndex + 1);
                    else
                        if (index != -1)
                            resultDepartments.Rows.InsertAt(row, index);
            }
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
