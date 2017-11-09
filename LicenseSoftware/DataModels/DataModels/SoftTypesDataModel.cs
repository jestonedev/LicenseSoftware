using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftTypesDataModel : DataModel
    {
        private static SoftTypesDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftTypes WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftTypes SET Deleted = 1 WHERE [ID SoftType] = @IDSoftType";
        private static string _insertQuery = @"INSERT INTO SoftTypes (SoftType) VALUES (@SoftType); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftTypes SET SoftType = @SoftType WHERE [ID SoftType] = @IDSoftType";
        private static string _tableName = "SoftTypes";

        private SoftTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID SoftType"] };
        }

        public static SoftTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftTypesDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftType", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить вид ПО из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftType softType)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект вида ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("SoftType", softType.SoftTypeName));
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftType", softType.IdSoftType));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о виде ПО. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftType softType)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект вида ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("SoftType", softType.SoftTypeName));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить вид ПО в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
