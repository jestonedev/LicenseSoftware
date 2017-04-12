using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;

namespace DataModels.DataModels
{
    public sealed class SoftwareDataModel : DataModel
    {
        private static SoftwareDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM Software WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE Software SET Deleted = 1 WHERE [ID Software] = @IDSoftware";
        private static string insertQuery = @"INSERT INTO Software
                            ([ID SoftType], [ID SoftMaker], Software, [Version])
                            VALUES (@IDSoftType, @IDSoftMaker, @Software, @Version); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string updateQuery = 
                @"UPDATE Software SET [ID SoftType] = @IDSoftType, [ID SoftMaker] = @IDSoftMaker, Software = @Software, [Version] = @Version 
                  WHERE [ID Software] = @IDSoftware";
        private static string tableName = "Software";

        public bool EditingNewRecord { get; set; }
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
                    return connection.SqlExecuteNonQuery(command);
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
                command.Parameters.Add(DBConnection.CreateParameter<string>("Version", software.Version));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSoftware", software.IdSoftware));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
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
            {
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
                command.Parameters.Add(DBConnection.CreateParameter<string>("Version", software.Version));

                try
                {                
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
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
