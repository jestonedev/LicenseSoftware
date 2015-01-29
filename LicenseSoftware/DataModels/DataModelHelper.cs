using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LicenseSoftware.CalcDataModels;
using System.Globalization;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels
{
    public static class DataModelHelper
    {
        public static IEnumerable<DataRow> FilterRows(DataTable table)
        {
            return from table_row in table.AsEnumerable()
                   where (table_row.RowState != DataRowState.Deleted) &&
                         (table_row.RowState != DataRowState.Detached)
                   select table_row;
        }

        public static IEnumerable<int> Intersect(IEnumerable<int> list1, IEnumerable<int> list2)
        {
            if (list1 != null && list2 != null)
                return list1.Intersect(list2).ToList();
            else
                if (list1 != null)
                    return list1;
                else
                    return list2;
        }

        public static IEnumerable<int> GetDepartmentSubUnits(int department)
        {
            DepartmentsDataModel departments = DepartmentsDataModel.GetInstance();
            List<int> departmentIDs = new List<int>();
            foreach (DataRow row in departments.Select().Rows)
                if (row["ID Parent Department"] != DBNull.Value && (int)row["ID Parent Department"] == department)
                    departmentIDs.Add((int)row["ID Department"]);
            foreach (int departmentID in departmentIDs)
                departmentIDs.Union(GetDepartmentSubUnits(departmentID));
            return departmentIDs;
        }
    }
}
