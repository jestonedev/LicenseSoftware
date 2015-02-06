using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data;
using System.Globalization;

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
            DataSetManager.AddRelation("SoftType", "ID SoftType", "Software", "ID SoftType", true);
            DataSetManager.AddRelation("SoftMakers", "ID SoftMaker", "Software", "ID SoftMaker", true);
            DataSetManager.AddRelation("SoftSuppliers", "ID Supplier", "SoftLicenses", "ID Supplier", true);
            DataSetManager.AddRelation("Software", "ID Software", "SoftLicenses", "ID Software", true);
            DataSetManager.AddRelation("SoftLicTypes", "ID LicType", "SoftLicenses", "ID LicType", true);
            DataSetManager.AddRelation("SoftLicDocTypes", "ID DocType", "SoftLicenses", "ID DocType", true);
            DataSetManager.AddRelation("SoftLicenses", "ID License", "SoftInstallations", "ID License", true);
            DataSetManager.AddRelation("SoftLicenses", "ID License", "SoftLicKeys", "ID License", true);
            DataSetManager.AddRelation("SoftInstallators", "ID Installator", "SoftInstallations", "ID Installator", true);
            DataSetManager.AddRelation("SoftLicKeys", "ID LicenseKey", "SoftInstallations", "ID LicenseKey", true);
            DataSetManager.AddRelation("Devices", "ID Device", "SoftInstallations", "ID Computer", true);
            DataSetManager.AddRelation("Departments", "ID Department", "SoftLicenses", "ID Department", true);
            DataSetManager.AddRelation("Departments", "ID Department", "Departments", "ID Parent Department", true);
            DataSetManager.AddRelation("Software", "ID Software", "SoftwareConcat", "ID Software", true);
            DataSetManager.AddRelation("SoftLicenses", "ID License", "LicensesConcat", "ID License", true);
        }

        private static void AddRelation(string master_table_name, string master_column_name, string slave_table_name, 
            string slave_column_name, bool create_constraints)
        {
            if (!dataSet.Tables.Contains(master_table_name))
                return;
            if (!dataSet.Tables.Contains(slave_table_name))
                return;
            if (!dataSet.Relations.Contains(master_table_name+"_"+slave_table_name))
            {
                DataRelation relation = new DataRelation(master_table_name + "_" + slave_table_name, 
                    dataSet.Tables[master_table_name].Columns[master_column_name], 
                    dataSet.Tables[slave_table_name].Columns[slave_column_name], create_constraints);
                dataSet.Relations.Add(relation);
            }
        }
    }
}
