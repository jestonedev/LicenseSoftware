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
    public sealed class SoftInstallationsDataModel : DataModel
    {
        private static SoftInstallationsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM SoftInstallations WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE SoftInstallations SET Deleted = 1 WHERE [ID Installation] = @IDInstallation";
        private static string insertQuery = @"INSERT INTO SoftInstallations
                            ([ID License], [ID Computer], InstallationDate, [ID LicenseKey], [ID Installator])
                            VALUES (@IDLicense, @IDComputer, @InstallationDate, @IDLicenseKey, @IDInstallator)";
        private static string updateQuery = @"UPDATE SoftInstallations SET [ID License] = @IDLicense, [ID Computer] = @IDComputer, 
                                              InstallationDate = @InstallationDate, [ID LicenseKey] = @IDLicenseKey, [ID Installator] = @IDInstallator
                                              WHERE [ID Installation] = @IDInstallation";
        private static string tableName = "SoftInstallations";

        public bool EditingNewRecord { get; set; }

        private SoftInstallationsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID Installation"] };
        }

        public static SoftInstallationsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftInstallationsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftInstallationsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallation", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить установку из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftInstallation softInstallation)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (softInstallation == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект установки программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", softInstallation.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDComputer", softInstallation.IdComputer));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("InstallationDate", softInstallation.InstallationDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicenseKey", softInstallation.IdLicenseKey));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallator", softInstallation.IdInstallator));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallation", softInstallation.IdInstallation));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные об установке программного обеспечения. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftInstallation softInstallation)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT @@IDENTITY";
                command.CommandText = insertQuery;
                if (softInstallation == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект установки программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", softInstallation.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDComputer", softInstallation.IdComputer));
                command.Parameters.Add(DBConnection.CreateParameter<DateTime?>("InstallationDate", softInstallation.InstallationDate));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicenseKey", softInstallation.IdLicenseKey));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallator", softInstallation.IdInstallator));

                try
                {
                    connection.SqlBeginTransaction();
                    connection.SqlModifyQuery(command);
                    DataTable last_id = connection.SqlSelectTable("last_id", last_id_command);
                    connection.SqlCommitTransaction();
                    if (last_id.Rows.Count == 0)
                    {
                        MessageBox.Show("Запрос не вернул идентификатор ключа", "Неизвестная ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return -1;
                    }
                    return Convert.ToInt32(last_id.Rows[0][0], CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить установку программного обеспечения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
