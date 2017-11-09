using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using LicenseSoftware.DataModels.DataModels;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels
{
    public static class DataModelHelper
    {
        public static IEnumerable<DataRow> FilterRows(DataTable table)
        {
            return from tableRow in table.AsEnumerable()
                   where (tableRow.RowState != DataRowState.Deleted) &&
                         (tableRow.RowState != DataRowState.Detached)
                   select tableRow;
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
            return list1 ?? list2;
        }

        public static IEnumerable<int> GetDepartmentSubunits(int department)
        {
            var departments = DepartmentsDataModel.GetInstance();
            var departmentIDs = new List<int>();
            foreach (DataRow row in departments.Select().Rows)
                if (row.RowState != DataRowState.Deleted &&
                    row.RowState != DataRowState.Detached && 
                    row["ID Parent Department"] != DBNull.Value && (int)row["ID Parent Department"] == department)
                    departmentIDs.Add((int)row["ID Department"]);
            var subUnits = new List<IEnumerable<int>>();
            foreach (var departmentId in departmentIDs)
                subUnits.Add(GetDepartmentSubunits(departmentId));
            foreach (var subUnit in subUnits)
                departmentIDs = departmentIDs.Union(subUnit).ToList();
            return departmentIDs;
        }

        public static IEnumerable<int> GetSoftwareIDsBySoftType(int idSoftType)
        {
            return from softwareRow in FilterRows(SoftwareDataModel.GetInstance().Select())
                   join softVersionRow in FilterRows(SoftVersionsDataModel.GetInstance().Select())
                   on softwareRow.Field<int>("ID Software") equals softVersionRow.Field<int>("ID Software")
                   where softwareRow.Field<int>("ID SoftType") == idSoftType
                   select softVersionRow.Field<int>("ID Version");
        }

        public static IEnumerable<int> GetSoftwareIDsBySoftMaker(int idSoftMaker)
        {
            return from softwareRow in FilterRows(SoftwareDataModel.GetInstance().Select())
                   join softVersionRow in FilterRows(SoftVersionsDataModel.GetInstance().Select())
                   on softwareRow.Field<int>("ID Software") equals softVersionRow.Field<int>("ID Software")
                   where softwareRow.Field<int>("ID SoftMaker") == idSoftMaker
                   select softVersionRow.Field<int>("ID Version");
        }

        public static IEnumerable<int> GetComputerIDsByDepartment(int idDepartment)
        {
            var childDepartments = from department in FilterRows(DepartmentsDataModel.GetInstance().Select())
                where department.Field<int?>("ID Parent Department") == idDepartment
                select department.Field<int>("ID Department");
            var computers = childDepartments.SelectMany(GetComputerIDsByDepartment);
            return computers.Concat(from computerRow in FilterRows(DevicesDataModel.GetInstance().Select())
                   where computerRow.Field<int>("ID Department") == idDepartment
                   select computerRow.Field<int>("ID Device"));
        }

        public static IEnumerable<int> GetLicenseIDsByCondition(Func<DataRow, bool> condition, EntityType entity)
        {
            var software = FilterRows(SoftwareDataModel.GetInstance().Select());
            var softVersions = FilterRows(SoftVersionsDataModel.GetInstance().Select());
            var departments = from departmentsRow in FilterRows(DepartmentsDataModel.GetInstance().SelectVisibleDepartments())
                              where departmentsRow.Field<bool>("AllowSelect")
                              select departmentsRow.Field<int>("ID Department");
            var licenses = from licensesRow in FilterRows(SoftLicensesDataModel.GetInstance().Select())
                           where departments.Contains(licensesRow.Field<int>("ID Department"))
                           select licensesRow;
            var result = from softwareRow in software
                         join softVersionRow in softVersions
                         on softwareRow.Field<int>("ID Software") equals softVersionRow.Field<int>("ID Software")
                         join licensesRow in licenses
                         on softVersionRow.Field<int>("ID Version") equals licensesRow.Field<int>("ID Version")
                         where entity == EntityType.Software ? condition(softwareRow) : 
                               entity == EntityType.SoftVersion ? condition(softVersionRow) :
                               entity == EntityType.License && condition(licensesRow)
                         select licensesRow.Field<int>("ID License");
            return result;
        }

        public static bool KeyIsFree(int idKey)
        {
            return !(from installationRow in FilterRows(SoftInstallationsDataModel.GetInstance().Select())
                   where installationRow.Field<int?>("ID LicenseKey") == idKey
                   select installationRow).Any();
        }

        /// <summary>
        /// Получает список идентификаторов неиспользованных ключей. Для корпоративных и электронных лицензий возвращает все ключи, для локальных только неиспользованные
        /// </summary>
        /// <param name="idLicense">Идентификатор лицензии</param>
        /// <returns>Список идентификаторов ключей</returns>
        public static IEnumerable<int> LicKeyIdsNotUsed(int idLicense)
        {
            return from licenseKeyRow in FilterRows(SoftLicKeysDataModel.GetInstance().Select())
                   join licenseRow in FilterRows(SoftLicensesDataModel.GetInstance().Select())
                   on licenseKeyRow.Field<int?>("ID License") equals licenseRow.Field<int?>("ID License")
                   join licenseTypeRow in FilterRows(SoftLicTypesDataModel.GetInstance().Select())
                   on licenseRow.Field<int?>("ID LicType") equals licenseTypeRow.Field<int?>("ID LicType")
                   join installationRow in FilterRows(SoftInstallationsDataModel.GetInstance().Select())
                   on licenseKeyRow.Field<int?>("ID LicenseKey") equals installationRow.Field<int?>("ID LicenseKey") into jg
                   from jgRow in jg.DefaultIfEmpty()
                   where licenseKeyRow.Field<int?>("ID License") == idLicense &&
                        (jgRow == null || licenseTypeRow.Field<bool?>("LicKeyDuplicateAllowed") == true)
                   select licenseKeyRow.Field<int>("ID LicenseKey");
        }
    }
}
