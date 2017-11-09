using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftSuppliersDataModel : DataModel
    {
        private static SoftSuppliersDataModel _dataModel;
        private static string _selectQuery = "SELECT * FROM SoftSuppliers WHERE Deleted = 0";
        private static string _deleteQuery = "UPDATE SoftSuppliers SET Deleted = 1 WHERE [ID Supplier] = @IDSupplier";
        private static string _insertQuery = @"INSERT INTO SoftSuppliers (Supplier) VALUES (@Supplier); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static string _updateQuery = @"UPDATE SoftSuppliers SET Supplier = @Supplier WHERE [ID Supplier] = @IDSupplier";
        private static string _tableName = "SoftSuppliers";

        private SoftSuppliersDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, _selectQuery, _tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID Supplier"] };
        }

        public static SoftSuppliersDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftSuppliersDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftSuppliersDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _deleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDSupplier", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить поставщика ПО из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftSupplier softSupplier)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _updateQuery;
                if (softSupplier == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект поставщика ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("Supplier", softSupplier.SoftSupplierName));
                command.Parameters.Add(DBConnection.CreateParameter("IDSupplier", softSupplier.IdSoftSupplier));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о поставщике ПО. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftSupplier softSupplier)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _insertQuery;
                if (softSupplier == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект поставщика ПО", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("Supplier", softSupplier.SoftSupplierName));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить поставщика ПО в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
