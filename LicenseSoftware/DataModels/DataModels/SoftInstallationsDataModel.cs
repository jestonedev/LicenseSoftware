using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftInstallationsDataModel : DataModel
    {
        private static SoftInstallationsDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftInstallations WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftInstallations SET Deleted = 1 WHERE [ID Installation] = @IDInstallation";
        private static string _insertQuery = @"INSERT INTO SoftInstallations
                            ([ID License], [ID Computer], InstallationDate, [ID LicenseKey], [ID Installator], [Description])
                            VALUES (@IDLicense, @IDComputer, @InstallationDate, @IDLicenseKey, @IDInstallator, @Description); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftInstallations SET [ID License] = @IDLicense, [ID Computer] = @IDComputer, 
                                              InstallationDate = @InstallationDate, [ID LicenseKey] = @IDLicenseKey, [ID Installator] = @IDInstallator, [Description] = @Description
                                              WHERE [ID Installation] = @IDInstallation";
        private static string _tableName = "SoftInstallations";

        public bool EditingNewRecord { get; set; }

        private SoftInstallationsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID Installation"] };
        }

        public static SoftInstallationsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftInstallationsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftInstallationsDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallation", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
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
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softInstallation == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект установки программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDLicense", softInstallation.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter("IDComputer", softInstallation.IdComputer));
                command.Parameters.Add(DBConnection.CreateParameter("InstallationDate", softInstallation.InstallationDate));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicenseKey", softInstallation.IdLicenseKey));
                command.Parameters.Add(DBConnection.CreateParameter("IDInstallator", softInstallation.IdInstallator));
                command.Parameters.Add(DBConnection.CreateParameter("IDInstallation", softInstallation.IdInstallation));
                command.Parameters.Add(DBConnection.CreateParameter("Description", softInstallation.Description));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
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
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softInstallation == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект установки программного обеспечения", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDLicense", softInstallation.IdLicense));
                command.Parameters.Add(DBConnection.CreateParameter("IDComputer", softInstallation.IdComputer));
                command.Parameters.Add(DBConnection.CreateParameter("InstallationDate", softInstallation.InstallationDate));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicenseKey", softInstallation.IdLicenseKey));
                command.Parameters.Add(DBConnection.CreateParameter("IDInstallator", softInstallation.IdInstallator));
                command.Parameters.Add(DBConnection.CreateParameter("Description", softInstallation.Description));

                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить установку программного обеспечения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
