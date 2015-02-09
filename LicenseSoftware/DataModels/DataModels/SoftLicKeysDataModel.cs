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
    public sealed class SoftLicKeysDataModel : DataModel
    {
        private static SoftLicKeysDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM SoftLicKeys WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE SoftLicKeys SET Deleted = 1 WHERE [ID LicenseKey] = @IDLicenseKey";
        private static string insertQuery = @"INSERT INTO SoftLicKeys
                            ([ID License], LicKey)
                            VALUES (@IDLicense, @LicKey); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string updateQuery = @"UPDATE SoftLicKeys SET [ID License] = @IDLicense, LicKey = @LicKey WHERE [ID LicenseKey] = @IDLicenseKey";
        private static string tableName = "SoftLicKeys";

        public bool EditingNewRecord { get; set; }

        private SoftLicKeysDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID LicenseKey"] };
        }

        public static SoftLicKeysDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicKeysDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftLicKeysDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicenseKey", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить лицензионный ключ из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftLicKey softLicKey)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (softLicKey == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект лицензионного ключа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", softLicKey.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter<string>("LicKey", softLicKey.LicKey));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicenseKey", softLicKey.IdLicenseKey));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о лицензионном ключе. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftLicKey softLicKey)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = insertQuery;
                if (softLicKey == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект лицензионного ключа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", softLicKey.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter<string>("LicKey", softLicKey.LicKey));

                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить лицензионный ключ в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
