using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class SoftLicensesDataModel : DataModel
    {
        private static SoftLicensesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM SoftLicenses WHERE Deleted = 0";
        private const string DeleteQuery = "UPDATE SoftLicenses SET Deleted = 1 WHERE [ID License] = @IDLicense";
        private const string InsertQuery = @"INSERT INTO SoftLicenses
                            ([ID Version], [ID LicType], [ID DocType], [ID Supplier], [ID Department], DocNumber, InstallationsCount, BuyLicenseDate, ExpireLicenseDate, Description)
                            VALUES (@IDVersion, @IDLicType, @IDDocType, @IDSupplier, @IDDepartment, @DocNumber, @InstallationsCount, @BuyLicenseDate, @ExpireLicenseDate, @Description); SELECT CONVERT(int, SCOPE_IDENTITY());";
        private static readonly string UpdateQuery = @"UPDATE SoftLicenses SET [ID Version] = @IDVersion, [ID LicType] = @IDLicType, [ID DocType] = @IDDocType,
                                                [ID Supplier] = @IDSupplier, [ID Department] = @IDDepartment, DocNumber = @DocNumber, 
                                                InstallationsCount = @InstallationsCount, BuyLicenseDate = @BuyLicenseDate, ExpireLicenseDate = @ExpireLicenseDate,
                                                Description = @Description
                                              WHERE [ID License] = @IDLicense";
        private const string TableName = "SoftLicenses";

        public bool EditingNewRecord { get; set; }

        private SoftLicensesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
        {
            EditingNewRecord = false;      
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID License"] };
            Table.Columns["InstallationsCount"].DefaultValue = 1;
        }

        public static SoftLicensesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static SoftLicensesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {
            return _dataModel ?? (_dataModel = new SoftLicensesDataModel(progressBar, incrementor));
        }

        public static int Delete(int id)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = DeleteQuery;
                command.Parameters.Add(DBConnection.CreateParameter<int?>("IDLicense", id));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось удалить лицензию на программное обеспечение из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Update(SoftLicense softLicense)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = UpdateQuery;
                if (softLicense == null)
                {
                    MessageBox.Show("В метод Update не передана ссылка на объект лицензии", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDVersion", softLicense.IdVersion));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicType", softLicense.IdLicType));
                command.Parameters.Add(DBConnection.CreateParameter("IDDocType", softLicense.IdDocType));
                command.Parameters.Add(DBConnection.CreateParameter("IDSupplier", softLicense.IdSupplier));
                command.Parameters.Add(DBConnection.CreateParameter("IDDepartment", softLicense.IdDepartment));
                command.Parameters.Add(DBConnection.CreateParameter("DocNumber", softLicense.DocNumber));
                command.Parameters.Add(DBConnection.CreateParameter("InstallationsCount", softLicense.InstallationsCount));
                command.Parameters.Add(DBConnection.CreateParameter("BuyLicenseDate", softLicense.BuyLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter("ExpireLicenseDate", softLicense.ExpireLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter("Description", softLicense.Description));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicense", softLicense.IdLicense));
                try
                {
                    return connection.SqlExecuteNonQuery(command);
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось изменить данные о лицензии на программное обеспечение. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }

        public static int Insert(SoftLicense softLicense)
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = InsertQuery;
                if (softLicense == null)
                {
                    MessageBox.Show("В метод Insert не передана ссылка на объект здания", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
                command.Parameters.Add(DBConnection.CreateParameter("IDVersion", softLicense.IdVersion));
                command.Parameters.Add(DBConnection.CreateParameter("IDLicType", softLicense.IdLicType));
                command.Parameters.Add(DBConnection.CreateParameter("IDDocType", softLicense.IdDocType));
                command.Parameters.Add(DBConnection.CreateParameter("IDSupplier", softLicense.IdSupplier));
                command.Parameters.Add(DBConnection.CreateParameter("IDDepartment", softLicense.IdDepartment));
                command.Parameters.Add(DBConnection.CreateParameter("DocNumber", softLicense.DocNumber));
                command.Parameters.Add(DBConnection.CreateParameter("InstallationsCount", softLicense.InstallationsCount));
                command.Parameters.Add(DBConnection.CreateParameter("BuyLicenseDate", softLicense.BuyLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter("ExpireLicenseDate", softLicense.ExpireLicenseDate));
                command.Parameters.Add(DBConnection.CreateParameter("Description", softLicense.Description));
                try
                {
                    return Convert.ToInt32(connection.SqlExecuteScalar(command), CultureInfo.InvariantCulture);
                }
                catch (SqlException e)
                {
                    connection.SqlRollbackTransaction();
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture, 
                        "Не удалось добавить лицензию на программное обеспечение в базу данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return -1;
                }
            }
        }
    }
}
