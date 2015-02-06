﻿using System;
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

        public static IEnumerable<DataRow> FilterRows(DataTable table, EntityType entity, int? idObject)
        {
            return from row in FilterRows(table)
                   where entity == EntityType.Unknown ? true :
                         entity == EntityType.Software ? row.Field<int?>("ID Software") == idObject :
                         entity == EntityType.License ? row.Field<int?>("ID License") == idObject :
                         entity == EntityType.Installation ? row.Field<int?>("ID Installation") == idObject : false
                   select row;
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
            IEnumerable<int> departmentIDs = new List<int>();
            foreach (DataRow row in departments.Select().Rows)
                if (row.RowState != DataRowState.Deleted &&
                    row.RowState != DataRowState.Detached && 
                    row["ID Parent Department"] != DBNull.Value && (int)row["ID Parent Department"] == department)
                    ((List<int>)departmentIDs).Add((int)row["ID Department"]);
            List<IEnumerable<int>> subUnits = new List<IEnumerable<int>>();
            foreach (int departmentID in departmentIDs)
                subUnits.Add(GetDepartmentSubUnits(departmentID));
            foreach (IEnumerable<int> subUnit in subUnits)
                departmentIDs = departmentIDs.Union(subUnit);
            return departmentIDs;
        }

        public static IEnumerable<int> GetSoftwareIDsBySoftType(int idSoftType)
        {
            return from software_row in DataModelHelper.FilterRows(SoftwareDataModel.GetInstance().Select())
                   where software_row.Field<int>("ID SoftType") == idSoftType
                   select software_row.Field<int>("ID Software");
        }

        public static IEnumerable<int> GetSoftwareIDsBySoftMaker(int idSoftMaker)
        {
            return from software_row in DataModelHelper.FilterRows(SoftwareDataModel.GetInstance().Select())
                   where software_row.Field<int>("ID SoftMaker") == idSoftMaker
                   select software_row.Field<int>("ID Software");
        }

        public static IEnumerable<int> GetComputerIDsByDepartment(int idDepartment)
        {
            return from computer_row in DataModelHelper.FilterRows(DevicesDataModel.GetInstance().Select())
                   where computer_row.Field<int>("ID Department") == idDepartment
                   select computer_row.Field<int>("ID Device");
        }

        public static IEnumerable<int> GetLicenseIDsByCondition(Func<DataRow, bool> condition, EntityType entity)
        {
            var software = DataModelHelper.FilterRows(SoftwareDataModel.GetInstance().Select());
            var departments = from departments_row in DataModelHelper.FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                              where departments_row.Field<bool>("AllowSelect")
                              select departments_row.Field<int>("ID Department");
            var licenses = from licenses_row in DataModelHelper.FilterRows(SoftLicensesDataModel.GetInstance().Select())
                           where departments.Contains(licenses_row.Field<int>("ID Department"))
                           select licenses_row;
            var result = from software_row in software
                         join licenses_row in licenses
                         on software_row.Field<int>("ID Software") equals licenses_row.Field<int>("ID Software")
                         where entity == EntityType.Software ? condition(software_row) : (entity == EntityType.License ? condition(licenses_row) : false)
                         select licenses_row.Field<int>("ID License");
            return result;
        }

        public static bool KeyIsFree(int idKey)
        {
            return (from installation_row in DataModelHelper.FilterRows(SoftInstallationsDataModel.GetInstance().Select())
                   where installation_row.Field<int?>("ID LicenseKey") == idKey
                   select installation_row).Count() == 0;
        }
    }
}