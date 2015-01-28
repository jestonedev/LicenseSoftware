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
    public sealed class SoftwareDataModel : DataModel
    {
        private static SoftwareDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM Software WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE Software SET Deleted = 1 WHERE [ID Software] = @IDSoftware";
        private static string insertQuery = @"INSERT INTO Software
                            ([ID SoftType], [ID SoftMaker], Software)
                            VALUES (@IDSoftType, @IDSoftMaker, @Software)";
        private static string updateQuery = 
                @"UPDATE Software SET [ID SoftType] = @IDSoftType, [ID SoftMaker] = @IDSoftMaker, Software = @Software WHERE [ID Software] = @IDSoftware";
        private static string tableName = "Software";

        private SoftwareDataModel(ToolStripProgressBar progressBar, int incrementor): base(progressBar, incrementor, selectQuery, tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID Software"] };
        }

        public static SoftwareDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftwareDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftwareDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftware", id));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить программное обеспечение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(Software software)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (software == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftType", software.IdSoftType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftMaker", software.IdSoftMaker));
                command.Parameters.Add(DBConnection.CreateParameter<string>("Software", software.SoftwareName));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftware", software.IdSoftware));
                try
                {
                    return connection.SqlModifyQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о программном обеспечении. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(Software software)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            using (DbCommand last_id_command = DBConnection.CreateCommand())
            {
                last_id_command.CommandText = "SELECT @@IDENTITY";
                command.CommandText = insertQuery;
                if (software == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IdSoftType", software.IdSoftType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IdSoftMaker", software.IdSoftMaker));
                command.Parameters.Add(DBConnection.CreateParameter<string>("Software", software.SoftwareName));

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
                        "Не удалось добавить программное обеспечение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
