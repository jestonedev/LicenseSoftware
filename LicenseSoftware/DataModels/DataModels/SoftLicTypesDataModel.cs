using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftLicTypesDataModel : DataModel
    {
        private static SoftLicTypesDataModel _dataModel;
        private const string SelectQuery = "SELECT [ID LicType], LicType, LicKeyDuplicateAllowed FROM SoftLicTypes WHERE Deleted = 0";
        private const string DeleteQuery = "UPDATE SoftLicTypes SET Deleted = 1 WHERE [ID LicType] = @IDLicType";
        private const string InsertQuery = @"INSERT INTO SoftLicTypes (LicType, LicKeyDuplicateAllowed) VALUES (@LicType, @LicKeyDuplicateAllowed); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private const string UpdateQuery = @"UPDATE SoftLicTypes SET LicType = @LicType, LicKeyDuplicateAllowed = @LicKeyDuplicateAllowed WHERE [ID LicType] = @IDLicType";
        private const string TableName = "SoftLicTypes";

        private SoftLicTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID LicType"] };
        }

        public static SoftLicTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftLicTypesDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = DeleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicType", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить вид лицензии на ПО из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftLicType softLicType)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = UpdateQuery;
                if (softLicType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект вида лицензии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("LicType", softLicType.LicType));
                command.Parameters.Add(DBConnection.CreateParameter("LicKeyDuplicateAllowed", softLicType.LicKeyDuplicateAllowed));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicType", softLicType.IdLicType));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о виде лицензии. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftLicType softLicType)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = InsertQuery;
                if (softLicType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект вида лицензии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("LicType", softLicType.LicType));
                command.Parameters.Add(DBConnection.CreateParameter("LicKeyDuplicateAllowed", softLicType.LicKeyDuplicateAllowed));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить вид лицензии в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
