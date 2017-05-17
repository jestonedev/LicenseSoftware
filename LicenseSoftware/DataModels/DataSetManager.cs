using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Globalization;
using DataModels.DataModels;

namespace LicenseSoftware.DataModels
{
    public static class DataSetManager
    {
        private static DataSet dataSet = new DataSet();

        public static DataSet DataSet { get { return dataSet; }}

        public static void AddModel(DataModel model)
        {
            if (model == null)
                throw new DataModelException("DataSetManager: Не передана ссылка на модель данных");
            DataTable table = model.Select();
            if (!dataSet.Tables.Contains(table.TableName))
                dataSet.Tables.Add(table);
            RebuildRelations();
        }

        public static void AddTable(DataTable table)
        {
            if (table == null)
                throw new DataModelException("DataSetManager: Не передана ссылка на таблицу");
            if (!dataSet.Tables.Contains(table.TableName))
                dataSet.Tables.Add(table);
            RebuildRelations();
        }

        private static void RebuildRelations()
        {
            AddRelation("SoftType", "ID SoftType", "Software", "ID SoftType", true);
            AddRelation("SoftMakers", "ID SoftMaker", "Software", "ID SoftMaker", true);
            AddRelation("SoftSuppliers", "ID Supplier", "SoftLicenses", "ID Supplier", true);
            AddRelation("SoftLicTypes", "ID LicType", "SoftLicenses", "ID LicType", true);
            AddRelation("SoftLicDocTypes", "ID DocType", "SoftLicenses", "ID DocType", true);
            AddRelation("SoftLicenses", "ID License", "SoftInstallations", "ID License", true);
            AddRelation("SoftLicenses", "ID License", "SoftLicKeys", "ID License", true);
            AddRelation("SoftInstallators", "ID Installator", "SoftInstallations", "ID Installator", true);
            AddRelation("SoftLicKeys", "ID LicenseKey", "SoftInstallations", "ID LicenseKey", true);
            AddRelation("Devices", "ID Device", "SoftInstallations", "ID Computer", true);
            AddRelation("Departments", "ID Department", "SoftLicenses", "ID Department", true);
            AddRelation("Departments", "ID Department", "Departments", "ID Parent Department", true);
            AddRelation("Software", "ID Software", "SoftVersions", "ID Software", true);
            AddRelation("SoftVersions", "ID Version", "SoftwareConcat", "ID Version", true);
            AddRelation("SoftLicenses", "ID License", "LicensesConcat", "ID License", true);
        }

        private static readonly object LockObj = new object();

        private static void AddRelation(string masterTableName, string masterColumnName, string slaveTableName, 
            string slaveColumnName, bool createConstraints)
        {
            lock (LockObj)
            {
                if (!dataSet.Tables.Contains(masterTableName))
                    return;
                if (!dataSet.Tables.Contains(slaveTableName))
                    return;
                if (!dataSet.Relations.Contains(masterTableName + "_" + slaveTableName))
                {
                    var relation = new DataRelation(masterTableName + "_" + slaveTableName,
                        dataSet.Tables[masterTableName].Columns[masterColumnName],
                        dataSet.Tables[slaveTableName].Columns[slaveColumnName], createConstraints);
                    dataSet.Relations.Add(relation);
                }   
            }
        }
    }
}
