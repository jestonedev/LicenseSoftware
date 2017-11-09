using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftVersionsDataModel : DataModel
    {
        private static SoftVersionsDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM SoftVersions WHERE Deleted = 0";
        private const string DeleteQuery = "UPDATE SoftVersions SET Deleted = 1 WHERE [ID Version] = @IDVersion";
        private const string InsertQuery = @"INSERT INTO SoftVersions
                            ([ID Software], Version)
                            VALUES (@IDSoftware, @Version); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private const string UpdateQuery = @"UPDATE SoftVersions SET [ID Software] = @IDSoftware, Version = @Version WHERE [ID Version] = @IDVersion";
        private const string TableName = "SoftVersions";

        public bool EditingNewRecord { get; set; }

        private SoftVersionsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID Version"] };
        }

        public static SoftVersionsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftVersionsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftVersionsDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = DeleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDVersion", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить лицензионный ключ из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftVersion softVersion)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = UpdateQuery;
                if (softVersion == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект лицензионного ключа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftware", softVersion.IdSoftware));
                command.Parameters.Add(DBConnection.CreateParameter("Version", softVersion.Version));
                command.Parameters.Add(DBConnection.CreateParameter("IDVersion", softVersion.IdVersion));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о лицензионном ключе. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftVersion softVersion)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = InsertQuery;
                if (softVersion == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект лицензионного ключа", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftware", softVersion.IdSoftware));
                command.Parameters.Add(DBConnection.CreateParameter("Version", softVersion.Version));

                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить лицензионный ключ в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
