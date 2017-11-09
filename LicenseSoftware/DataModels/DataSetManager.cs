using System.Data;
using LicenseSoftware.DataModels.DataModels;

namespace LicenseSoftware.DataModels
{
    public static class DataSetManager
    {
        private static DataSet _dataSet = new DataSet();

        public static DataSet DataSet { get { return _dataSet; }}

        public static void AddModel(DataModel model)
        {
            if (model == null)
                throw new DataModelException("DataSetManager: Не передана ссылка на модель данных");
            var table = model.Select();
            if (!_dataSet.Tables.Contains(table.TableName))
                _dataSet.Tables.Add(table);
            RebuildRelations();
        }

        public static void AddTable(DataTable table)
        {
            if (table == null)
                throw new DataModelException("DataSetManager: Не передана ссылка на таблицу");
            if (!_dataSet.Tables.Contains(table.TableName))
                _dataSet.Tables.Add(table);
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
                if (!_dataSet.Tables.Contains(masterTableName))
                    return;
                if (!_dataSet.Tables.Contains(slaveTableName))
                    return;
                if (!_dataSet.Relations.Contains(masterTableName + "_" + slaveTableName))
                {
                    var relation = new DataRelation(masterTableName + "_" + slaveTableName,
                        _dataSet.Tables[masterTableName].Columns[masterColumnName],
                        _dataSet.Tables[slaveTableName].Columns[slaveColumnName], createConstraints);
                    _dataSet.Relations.Add(relation);
                }   
            }
        }
    }
}
