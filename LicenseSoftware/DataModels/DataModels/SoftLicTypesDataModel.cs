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
    public sealed class SoftLicTypesDataModel : DataModel
    {
        private static SoftLicTypesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM SoftLicTypes WHERE Deleted = 0";
        private static string deleteQuery = "UPDATE SoftLicTypes SET Deleted = 1 WHERE [ID LicType] = @IDLicType";
        private static string insertQuery = @"INSERT INTO SoftLicTypes (LicType) VALUES (@LicType); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string updateQuery = @"UPDATE SoftLicTypes SET LicType = @LicType WHERE [ID LicType] = @IDLicType";
        private static string tableName = "SoftLicTypes";

        private SoftLicTypesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID LicType"] };
        }

        public static SoftLicTypesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicTypesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new SoftLicTypesDataModel(progressBar, incrementor);
            return dataModel;
        }

        public static int Delete(int id)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicType", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить вид лицензии на ПО из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftLicType softLicType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = updateQuery;
                if (softLicType == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект вида лицензии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("LicType", softLicType.LicType));
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicType", softLicType.IdLicType));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о виде лицензии. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftLicType softLicType)
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = insertQuery;
                if (softLicType == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект вида лицензии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter<string>("LicType", softLicType.LicType));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить вид лицензии в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
