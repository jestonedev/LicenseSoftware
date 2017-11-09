using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftMakersDataModel : DataModel
    {
        private static SoftMakersDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftMakers WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftMakers SET Deleted = 1 WHERE [ID SoftMaker] = @IDSoftMaker";
        private static string _insertQuery = @"INSERT INTO SoftMakers (SoftMaker) VALUES (@SoftMaker); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftMakers SET SoftMaker = @SoftMaker WHERE [ID SoftMaker] = @IDSoftMaker";
        private static string _tableName = "SoftMakers";

        private SoftMakersDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID SoftMaker"] };
        }

        public static SoftMakersDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftMakersDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftMakersDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftMaker", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить разработчика ПО из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftMaker softMaker)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softMaker == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект разработчика ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("SoftMaker", softMaker.SoftMakerName));
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftMaker", softMaker.IdSoftMaker));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о разработчике ПО. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftMaker softMaker)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softMaker == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект разработчика ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("SoftMaker", softMaker.SoftMakerName));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить разработчика ПО в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
