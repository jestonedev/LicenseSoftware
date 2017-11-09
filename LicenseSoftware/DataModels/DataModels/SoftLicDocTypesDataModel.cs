using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftLicDocTypesDataModel : DataModel
    {
        private static SoftLicDocTypesDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftLicDocTypes WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftLicDocTypes SET Deleted = 1 WHERE [ID DocType] = @IDDocType";
        private static string _insertQuery = @"INSERT INTO SoftLicDocTypes (DocType) VALUES (@DocType); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftLicDocTypes SET DocType = @DocType WHERE [ID DocType] = @IDDocType";
        private static string _tableName = "SoftLicDocTypes";

        private SoftLicDocTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID DocType"] };
        }

        public static SoftLicDocTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicDocTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftLicDocTypesDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDDocType", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить вид документа-основания на приобретение лицензии из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftLicDocType softLicDocType)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softLicDocType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект документа-основания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("DocType", softLicDocType.DocType));
                command.Parameters.Add(DBConnection.CreateParameter("IDDocType", softLicDocType.IdDocType));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о документе-основании. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftLicDocType softLicDocType)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softLicDocType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект документа-основания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("DocType", softLicDocType.DocType));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить документ-основание в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
