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
    public sealed class SoftLicensesDataModel : DataModel
    {
        private static SoftLicensesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM SoftLicenses WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE SoftLicenses SET Deleted = 1 WHERE [ID License] = @IDLicense";
        private static string insertQuery = @"INSERT INTO SoftLicenses
                            ([ID Software], [ID LicType], [ID DocType], [ID Supplier], [ID Department], DocNumber, InstallationsCount, BuyLicenseDate, ExpireLicenseDate, Description)
                            VALUES (@IDSoftware, @IDLicType, @IDDocType, @IDSupplier, @IDDepartment, @DocNumber, @InstallationsCount, @BuyLicenseDate, @ExpireLicenseDate, @Description); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string updateQuery = @"UPDATE SoftLicenses SET [ID Software] = @IDSoftware, [ID LicType] = @IDLicType, [ID DocType] = @IDDocType,
                                                [ID Supplier] = @IDSupplier, [ID Department] = @IDDepartment, DocNumber = @DocNumber, 
                                                InstallationsCount = @InstallationsCount, BuyLicenseDate = @BuyLicenseDate, ExpireLicenseDate = @ExpireLicenseDate,
                                                Description = @Description
                                              WHERE [ID License] = @IDLicense";
        private static string tableName = "SoftLicenses";

        public bool EditingNewRecord { get; set; }

        private SoftLicensesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID License"] };
            Table.Columns["InstallationsCount"].DefaultValue = 1;
        }

        public static SoftLicensesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicensesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftLicensesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить лицензию на программное обеспечение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftLicense softLicense)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (softLicense == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект лицензии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftware", softLicense.IdSoftware));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicType", softLicense.IdLicType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDDocType", softLicense.IdDocType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSupplier", softLicense.IdSupplier));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDDepartment", softLicense.IdDepartment));
                command.Parameters.Add(DBConnection.CreateParameter<string>("DocNumber", softLicense.DocNumber));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("InstallationsCount", softLicense.InstallationsCount));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("BuyLicenseDate", softLicense.BuyLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("ExpireLicenseDate", softLicense.ExpireLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("Description", softLicense.Description));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", softLicense.IdLicense));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о лицензии на программное обеспечение. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftLicense softLicense)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = insertQuery;
                if (softLicense == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект здания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftware", softLicense.IdSoftware));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicType", softLicense.IdLicType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDDocType", softLicense.IdDocType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSupplier", softLicense.IdSupplier));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDDepartment", softLicense.IdDepartment));
                command.Parameters.Add(DBConnection.CreateParameter<string>("DocNumber", softLicense.DocNumber));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("InstallationsCount", softLicense.InstallationsCount));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("BuyLicenseDate", softLicense.BuyLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("ExpireLicenseDate", softLicense.ExpireLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter<string>("Description", softLicense.Description));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить лицензию на программное обеспечение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
