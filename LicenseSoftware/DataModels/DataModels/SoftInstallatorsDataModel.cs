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
    public sealed class SoftInstallatorsDataModel : DataModel
    {
        private static SoftInstallatorsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM SoftInstallators WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE SoftInstallators SET Deleted = 1 WHERE [ID Installator] = @IDInstallator";
        private static string insertQuery = @"INSERT INTO SoftInstallators
                            (FullName, Profession, Inactive)
                            VALUES (@FullName, @Profession, @Inactive); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string updateQuery = @"UPDATE SoftInstallators SET FullName = @FullName, Profession = @Profession, Inactive = @Inactive
                                              WHERE [ID Installator] = @IDInstallator";
        private static string tableName = "SoftInstallators";

        private SoftInstallatorsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        { 
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID Installator"] };
            Table.Columns["Inactive"].DefaultValue = false;
        }

        public static SoftInstallatorsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftInstallatorsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftInstallatorsDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
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
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (softInstallator == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект установщика", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("FullName", softInstallator.FullName));
                command.Parameters.Add(DBConnection.CreateParameter<string>("Profession", softInstallator.Profession));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("Inactive", softInstallator.Inactive));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDInstallator", softInstallator.IdInstallator));
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
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = insertQuery;
                if (softInstallator == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект установщика", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("FullName", softInstallator.FullName));
                command.Parameters.Add(DBConnection.CreateParameter<string>("Profession", softInstallator.Profession));
                command.Parameters.Add(DBConnection.CreateParameter<bool?>("Inactive", softInstallator.Inactive));
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
