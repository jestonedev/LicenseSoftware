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
    public sealed class SoftLicDocTypesDataModel : DataModel
    {
        private static SoftLicDocTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM SoftLicDocTypes WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE SoftLicDocTypes SET Deleted = 1 WHERE [ID DocType] = @IDDocType";
        private static string insertQuery = @"INSERT INTO SoftLicDocTypes (DocType) VALUES (@DocType); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string updateQuery = @"UPDATE SoftLicDocTypes SET DocType = @DocType WHERE [ID DocType] = @IDDocType";
        private static string tableName = "SoftLicDocTypes";

        private SoftLicDocTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID DocType"] };
        }

        public static SoftLicDocTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicDocTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftLicDocTypesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
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
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (softLicDocType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект документа-основания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("DocType", softLicDocType.DocType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDDocType", softLicDocType.IdDocType));
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
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = insertQuery;
                if (softLicDocType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект документа-основания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("DocType", softLicDocType.DocType));
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
