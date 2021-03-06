﻿using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftLicKeysDataModel : DataModel
    {
        private static SoftLicKeysDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftLicKeys WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftLicKeys SET Deleted = 1 WHERE [ID LicenseKey] = @IDLicenseKey";
        private static string _insertQuery = @"INSERT INTO SoftLicKeys
                            ([ID License], LicKey)
                            VALUES (@IDLicense, @LicKey); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftLicKeys SET [ID License] = @IDLicense, LicKey = @LicKey WHERE [ID LicenseKey] = @IDLicenseKey";
        private static string _tableName = "SoftLicKeys";

        public bool EditingNewRecord { get; set; }

        private SoftLicKeysDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID LicenseKey"] };
        }

        public static SoftLicKeysDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicKeysDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftLicKeysDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
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
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softLicKey == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект лицензионного ключа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDLicense", softLicKey.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter("LicKey", softLicKey.LicKey));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicenseKey", softLicKey.IdLicenseKey));
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
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softLicKey == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект лицензионного ключа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDLicense", softLicKey.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter("LicKey", softLicKey.LicKey));

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
