using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftwareDataModel : DataModel
    {
        private static SoftwareDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM Software WHERE Deleted = 0";
        private const string DeleteQuery = "UPDATE Software SET Deleted = 1 WHERE [ID Software] = @IDSoftware";
        private const string InsertQuery = @"INSERT INTO Software
                            ([ID SoftType], [ID SoftMaker], Software)
                            VALUES (@IDSoftType, @IDSoftMaker, @Software); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private const string UpdateQuery = @"UPDATE Software SET [ID SoftType] = @IDSoftType, [ID SoftMaker] = @IDSoftMaker, Software = @Software 
                  WHERE [ID Software] = @IDSoftware";
        private const string TableName = "Software";

        public bool EditingNewRecord { get; set; }
        private SoftwareDataModel(ToolStripProgressBar progressBar, int incrementor): base(progressBar, incrementor, SelectQuery, TableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID Software"] };
        }

        public static SoftwareDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftwareDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftwareDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = DeleteQuery;
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
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = UpdateQuery;
                if (software == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftType", software.IdSoftType));
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftMaker", software.IdSoftMaker));
                command.Parameters.Add(DBConnection.CreateParameter("Software", software.SoftwareName));
                command.Parameters.Add(DBConnection.CreateParameter("IDSoftware", software.IdSoftware));
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
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = InsertQuery;
                if (software == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IdSoftType", software.IdSoftType));
                command.Parameters.Add(DBConnection.CreateParameter("IdSoftMaker", software.IdSoftMaker));
                command.Parameters.Add(DBConnection.CreateParameter("Software", software.SoftwareName));

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
