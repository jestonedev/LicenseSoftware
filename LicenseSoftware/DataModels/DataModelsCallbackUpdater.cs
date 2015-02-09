using LicenseSoftware.CalcDataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LicenseSoftware.DataModels
{
    public sealed class DataModelsCallbackUpdater
    {
        private static DataModelsCallbackUpdater instance;
        private static string query = @"SELECT [ID Record], [Table], [ID Key], [Field Name], [Field New Value], [Operation Type] 
                                        FROM Log WHERE [ID Record] > @IDRecord AND ([Operation Type] = 'UPDATE' OR ([Operation Type] IN ('DELETE','INSERT') AND ([User Name] <> @UserName)))";
        private static string initQuery = @"SELECT ISNULL(MAX([ID Record]), 0) AS [ID Record], suser_sname() AS [User Name] FROM Log";
        private int idRecord = -1;
        private string userName = "";
        private DataRow updRow = null;

        private DataModelsCallbackUpdater()
        {
        }

        public void Initialize()
        {
            using (DBConnection connection = new DBConnection())
            using (DbCommand command = DBConnection.CreateCommand())
            {
                command.CommandText = initQuery;
                try
                {
                    DataTable table = connection.SqlSelectTable("table", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Не удалось инициализировать DataModelCallbackUpdater", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                        Application.Exit();
                    }
                    idRecord = Convert.ToInt32(table.Rows[0]["ID Record"].ToString(), CultureInfo.InvariantCulture);
                    userName = table.Rows[0]["User Name"].ToString();
                }
                catch (SqlException e)
                {
                    MessageBox.Show(String.Format(CultureInfo.InvariantCulture,
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    Application.Exit();
                }
            }
        }

        public void Run()
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                DataTable tableCacheLvl1 = new DataTable("tableCacheLvl1");
                DataTable tableCacheLvl2 = new DataTable("tableCacheLvl2");
                tableCacheLvl1.Locale = CultureInfo.InvariantCulture;
                tableCacheLvl2.Locale = CultureInfo.InvariantCulture;
                InitializeColumns(tableCacheLvl1);
                InitializeColumns(tableCacheLvl2);
                while (true)
                {
                    //Пробуем обновить модель из кэша
                    tableCacheLvl2.Clear();
                    context.Send(__ =>
                    {
                        DataTable workTable = tableCacheLvl1;
                        foreach (DataRow row in workTable.Rows)
                        {
                            if (!UpdateModelFromRow(row))
                                tableCacheLvl2.Rows.Add(RowToCacheObject(row));
                        }
                    }, null);
                    //Переносим данные из кэша 2-го уровня в первый
                    tableCacheLvl1.Clear();
                    foreach (DataRow row in tableCacheLvl2.Rows)
                        tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                    //Обновляем модель из базы
                    using (DBConnection connection = new DBConnection())
                    using (DbCommand command = DBConnection.CreateCommand())
                    {
                        command.CommandText = query;
                        command.Parameters.Add(DBConnection.CreateParameter<int>("IDRecord", idRecord));
                        command.Parameters.Add(DBConnection.CreateParameter<string>("UserName", userName));
                        try
                        {
                            DataTable tableDB = connection.SqlSelectTable("tableDB", command);
                            tableDB.Locale = CultureInfo.InvariantCulture;
                            context.Send(__ =>
                            {
                                DataTable workTable = tableDB;
                                foreach (DataRow row in workTable.Rows)
                                {
                                    if (!UpdateModelFromRow(row))
                                        tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                                }
                            }, null);
                            if (tableDB.Rows.Count > 0)
                                idRecord = Convert.ToInt32(tableDB.Rows[tableDB.Rows.Count - 1]["ID Record"].ToString(), CultureInfo.InvariantCulture);
                        }
                        catch (SqlException)
                        {
                            //При ошибке загрузки из БД мы просто игнорируем и не производим обновление локальной модели до тех пор, пока связь не установится
                        }
                    }
                    //Обновление делаем примерно каждые DataModelsCallbackUpdateTimeout милисекунд
                    Thread.Sleep(LicenseSoftwareSettings.DataModelsCallbackUpdateTimeout);
                }
            }, null);
        }

        private bool UpdateModelFromRow(DataRow row)
        {
            string table = row["Table"].ToString();
            int id_key = Convert.ToInt32(row["ID Key"].ToString(), CultureInfo.InvariantCulture);
            string field_name = row["Field Name"].ToString();
            string field_value = row["Field New Value"].ToString();
            string operation_type = row["Operation Type"].ToString();
            //Если таблица не загружена, то у пользователя просто нет необходимых прав. Игнорируем ее и возвращаем true
            if (!DataSetManager.DataSet.Tables.Contains(table))
                return true;
            DataTable updTable = DataSetManager.DataSet.Tables[table];
            //Ищем строку для обновления
            if (!((updRow != null)
                && (updRow.RowState != DataRowState.Deleted)
                && (updRow.RowState != DataRowState.Detached)
                && (updRow.Table.TableName == table) 
                && (updRow.Table.PrimaryKey.Length > 0)
                && (Convert.ToInt32(updRow[updRow.Table.PrimaryKey[0].ColumnName], CultureInfo.InvariantCulture) == id_key)))
            {
                //Если строка не закэширована, или закэширована не та строка, то надо найти и закэшировать строку по имени таблицы и id_key
                updRow = updTable.Rows.Find(id_key);
            }
            //Если строка в представлении пользователя существует, но помечена как удаленная, то игнорировать ее
            if (updRow != null && (updRow.RowState == DataRowState.Deleted || updRow.RowState == DataRowState.Detached))
                return true;
            switch (operation_type)
            {
                case "INSERT":
                    //Если модель находится в режиме IsNewRecord, то вернуть false
                    if (EditingNewRecordModel(table))
                        return false;
                    //Если строки нет, то создаем новую
                    if (updRow == null)
                    {
                        updRow = updTable.NewRow();
                        updRow[updRow.Table.PrimaryKey[0].ColumnName] = id_key;
                        updRow.EndEdit();
                        updTable.Rows.Add(updRow);
                    }
                    SetValue(updRow, field_name, field_value, operation_type);
                    return true;
                case "UPDATE":
                    //Если строки нет, то игнорируем и возвращаем false, чтобы сохранить в кэш строку
                    if (updRow == null)
                        return false;
                    SetValue(updRow, field_name, field_value, operation_type);
                    return true;
                case "DELETE":
                    //Если строка не найдена, значит она уже удалена, возвращаем true
                    if (updRow == null)
                        return true;
                    updTable.Rows.Remove(updRow);
                    CalcDataModelsUpdate(table, field_name, operation_type);
                    return true;
                default:
                    return true;
            }
        }

        private static void CalcDataModelsUpdate(string table, string field_name, string operation_type)
        {
            switch (table)
            {
                case "Software":
                    if (CalcDataModelSoftwareConcat.HasInstance())
                        CalcDataModelSoftwareConcat.GetInstance().DefferedUpdate = true;
                    break;
                case "SoftLicenses":
                    if (CalcDataModelLicensesConcat.HasInstance())
                        CalcDataModelLicensesConcat.GetInstance().DefferedUpdate = true;
                    break;
            }
        }

        private static void SetValue(DataRow row, string field_name, string field_value, string operation_type)
        {
            // Если поле не найдено, то возможно оно новое в базе и надо его проигнорировать
            if (!row.Table.Columns.Contains(field_name))
                return;
            if (String.IsNullOrEmpty(field_value))
            {
                if (!row[field_name].Equals(DBNull.Value))
                {
                    row[field_name] = DBNull.Value;
                    CalcDataModelsUpdate(row.Table.TableName, field_name, operation_type);
                }
            }
            else
            {
                object value = DBNull.Value;
                if (row.Table.Columns[field_name].DataType == typeof(Boolean))
                    value = field_value == "1" ? true : false;
                else
                    value = Convert.ChangeType(field_value, row.Table.Columns[field_name].DataType, CultureInfo.InvariantCulture);
                if (!row[field_name].Equals(value))
                {
                    row[field_name] = value;
                    CalcDataModelsUpdate(row.Table.TableName, field_name, operation_type);
                }
            }
        }

        private static bool EditingNewRecordModel(string table)
        {
            switch (table)
            {
                case "Software":
                    return SoftwareDataModel.GetInstance().EditingNewRecord;
                case "SoftLicenses": 
                    return SoftLicensesDataModel.GetInstance().EditingNewRecord;
                case "SoftInstallations":
                    return SoftInstallationsDataModel.GetInstance().EditingNewRecord;
                default:
                    return false;
            }
        }

        private static void InitializeColumns(DataTable table)
        {
            table.Columns.Add("Table").DataType = typeof(string);
            table.Columns.Add("ID Key").DataType = typeof(int);
            table.Columns.Add("Field Name").DataType = typeof(string);
            table.Columns.Add("Field New Value").DataType = typeof(string);
            table.Columns.Add("Operation Type").DataType = typeof(string);
        }

        private static object[] RowToCacheObject(DataRow row)
        {
            return new object[] {
                row["Table"],
                row["ID Key"],
                row["Field Name"],
                row["Field New Value"],
                row["Operation Type"]
            };
        }

        public static DataModelsCallbackUpdater GetInstance()
        {
            if (instance == null)
                instance = new DataModelsCallbackUpdater();
            return instance;
        }
    }
}
