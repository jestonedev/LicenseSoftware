using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using LicenseSoftware.DataModels.CalcDataModels;
using LicenseSoftware.DataModels.DataModels;
using Settings;

namespace LicenseSoftware.DataModels
{
    public sealed class DataModelsCallbackUpdater
    {
        private static DataModelsCallbackUpdater _instance;
        private static string _query = @"SELECT [ID Record], [Table], [ID Key], [Field Name], [Field New Value], [Operation Type] 
                                        FROM Log WHERE [ID Record] > @IDRecord AND ([Operation Type] = 'UPDATE' OR ([Operation Type] IN ('DELETE','INSERT') AND ([User Name] <> @UserName)) OR [Table] IN ('Departments','Devices'))";
        private static string _initQuery = @"SELECT ISNULL(MAX([ID Record]), 0) AS [ID Record], suser_sname() AS [User Name] FROM Log";
        private int _idRecord = -1;
        private string _userName = "";
        private DataRow _updRow;

        private DataModelsCallbackUpdater()
        {
        }

        public void Initialize()
        {
            using (var connection = new DBConnection())
            using (var command = DBConnection.CreateCommand())
            {
                command.CommandText = _initQuery;
                try
                {
                    var table = connection.SqlSelectTable("table", command);
                    if (table.Rows.Count == 0)
                    {
                        MessageBox.Show("Не удалось инициализировать DataModelCallbackUpdater", "Неизвестная ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error,
                            MessageBoxDefaultButton.Button1);
                        Application.Exit();
                    }
                    _idRecord = Convert.ToInt32(table.Rows[0]["ID Record"].ToString(), CultureInfo.InvariantCulture);
                    _userName = table.Rows[0]["User Name"].ToString();
                }
                catch (SqlException e)
                {
                    MessageBox.Show(string.Format(CultureInfo.InvariantCulture,
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    Application.Exit();
                }
            }
        }

        public void Run()
        {
            var context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                var tableCacheLvl1 = new DataTable("tableCacheLvl1");
                var tableCacheLvl2 = new DataTable("tableCacheLvl2");
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
                        var workTable = tableCacheLvl1;
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
                    using (var connection = new DBConnection())
                    using (var command = DBConnection.CreateCommand())
                    {
                        command.CommandText = _query;
                        command.Parameters.Add(DBConnection.CreateParameter("IDRecord", _idRecord));
                        command.Parameters.Add(DBConnection.CreateParameter("UserName", _userName));
                        try
                        {
                            var tableDb = connection.SqlSelectTable("tableDB", command);
                            tableDb.Locale = CultureInfo.InvariantCulture;
                            context.Send(__ =>
                            {
                                var workTable = tableDb;
                                foreach (DataRow row in workTable.Rows)
                                {
                                    if (!UpdateModelFromRow(row))
                                        tableCacheLvl1.Rows.Add(RowToCacheObject(row));
                                }
                            }, null);
                            if (tableDb.Rows.Count > 0)
                                _idRecord = Convert.ToInt32(tableDb.Rows[tableDb.Rows.Count - 1]["ID Record"].ToString(), CultureInfo.InvariantCulture);
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
            var table = row["Table"].ToString();
            var idKey = Convert.ToInt32(row["ID Key"].ToString(), CultureInfo.InvariantCulture);
            var fieldName = row["Field Name"].ToString();
            var fieldValue = row["Field New Value"].ToString();
            var operationType = row["Operation Type"].ToString();
            //Если таблица не загружена, то у пользователя просто нет необходимых прав. Игнорируем ее и возвращаем true
            if (!DataSetManager.DataSet.Tables.Contains(table))
                return true;
            var updTable = DataSetManager.DataSet.Tables[table];
            //Ищем строку для обновления
            if (!((_updRow != null)
                && (_updRow.RowState != DataRowState.Deleted)
                && (_updRow.RowState != DataRowState.Detached)
                && (_updRow.Table.TableName == table) 
                && (_updRow.Table.PrimaryKey.Length > 0)
                && (Convert.ToInt32(_updRow[_updRow.Table.PrimaryKey[0].ColumnName], CultureInfo.InvariantCulture) == idKey)))
            {
                //Если строка не закэширована, или закэширована не та строка, то надо найти и закэшировать строку по имени таблицы и id_key
                _updRow = updTable.Rows.Find(idKey);
            }
            //Если строка в представлении пользователя существует, но помечена как удаленная, то игнорировать ее
            if (_updRow != null && (_updRow.RowState == DataRowState.Deleted || _updRow.RowState == DataRowState.Detached))
                return true;
            switch (operationType)
            {
                case "INSERT":
                    //Если модель находится в режиме IsNewRecord, то вернуть false
                    if (EditingNewRecordModel(table))
                        return false;
                    //Если строки нет, то создаем новую
                    if (_updRow == null)
                    {
                        _updRow = updTable.NewRow();
                        _updRow[_updRow.Table.PrimaryKey[0].ColumnName] = idKey;
                        _updRow.EndEdit();
                        updTable.Rows.Add(_updRow);
                    }
                    SetValue(_updRow, fieldName, fieldValue);
                    return true;
                case "UPDATE":
                    //Если строки нет, то игнорируем и возвращаем false, чтобы сохранить в кэш строку
                    if (_updRow == null)
                        return false;
                    SetValue(_updRow, fieldName, fieldValue);
                    return true;
                case "DELETE":
                    //Если строка не найдена, значит она уже удалена, возвращаем true
                    if (_updRow == null)
                        return true;
                    updTable.Rows.Remove(_updRow);
                    CalcDataModelsUpdate(table);
                    return true;
                default:
                    return true;
            }
        }

        private static void CalcDataModelsUpdate(string table)
        {
            switch (table)
            {
                case "Software":
                    if (CalcDataModelSoftwareConcat.HasInstance())
                        CalcDataModelSoftwareConcat.GetInstance().DefferedUpdate = true;
                    break;
                case "SoftVersions":
                    if (CalcDataModelSoftwareConcat.HasInstance())
                        CalcDataModelSoftwareConcat.GetInstance().DefferedUpdate = true;
                    break;
                case "SoftLicenses":
                    if (CalcDataModelLicensesConcat.HasInstance())
                        CalcDataModelLicensesConcat.GetInstance().DefferedUpdate = true;
                    break;
            }
        }

        private static void SetValue(DataRow row, string fieldName, string fieldValue)
        {
            // Если поле не найдено, то возможно оно новое в базе и надо его проигнорировать
            if (!row.Table.Columns.Contains(fieldName))
                return;
            if (string.IsNullOrEmpty(fieldValue))
            {
                if (row[fieldName].Equals(DBNull.Value)) return;
                row[fieldName] = DBNull.Value;
                CalcDataModelsUpdate(row.Table.TableName);
            }
            else
            {
                object value;
                if (row.Table.Columns[fieldName].DataType == typeof(bool))
                    value = fieldValue == "1";
                else
                    value = Convert.ChangeType(fieldValue, row.Table.Columns[fieldName].DataType, CultureInfo.InvariantCulture);
                if (row[fieldName].Equals(value)) return;
                row[fieldName] = value;
                CalcDataModelsUpdate(row.Table.TableName);
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
            return new[] {
                row["Table"],
                row["ID Key"],
                row["Field Name"],
                row["Field New Value"],
                row["Operation Type"]
            };
        }

        public static DataModelsCallbackUpdater GetInstance()
        {
            return _instance ?? (_instance = new DataModelsCallbackUpdater());
        }
    }
}
