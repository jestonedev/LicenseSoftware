using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftInstallatorsDataModel : DataModel
    {
        private static SoftInstallatorsDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftInstallators WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftInstallators SET Deleted = 1 WHERE [ID Installator] = @IDInstallator";
        private static string _insertQuery = @"INSERT INTO SoftInstallators
                            (FullName, Profession, Inactive)
                            VALUES (@FullName, @Profession, @Inactive); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftInstallators SET FullName = @FullName, Profession = @Profession, Inactive = @Inactive
                                              WHERE [ID Installator] = @IDInstallator";
        private static string _tableName = "SoftInstallators";

        private SoftInstallatorsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        { 
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID Installator"] };
            Table.Columns["Inactive"].DefaultValue = false;
        }

        public static SoftInstallatorsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftInstallatorsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftInstallatorsDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallator", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить установщика программное обеспечение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftInstallator softInstallator)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softInstallator == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект установщика", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("FullName", softInstallator.FullName));
                command.Parameters.Add(DBConnection.CreateParameter("Profession", softInstallator.Profession));
                command.Parameters.Add(DBConnection.CreateParameter("Inactive", softInstallator.Inactive));
                command.Parameters.Add(DBConnection.CreateParameter("IDInstallator", softInstallator.IdInstallator));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные об установщике программного обеспечения. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftInstallator softInstallator)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softInstallator == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект установщика", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("FullName", softInstallator.FullName));
                command.Parameters.Add(DBConnection.CreateParameter("Profession", softInstallator.Profession));
                command.Parameters.Add(DBConnection.CreateParameter("Inactive", softInstallator.Inactive));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить установщика программного обеспечения в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
