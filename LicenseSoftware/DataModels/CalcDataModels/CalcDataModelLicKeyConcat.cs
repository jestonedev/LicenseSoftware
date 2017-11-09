using System;
using System.Data;
using System.Globalization;
using System.Linq;
using LicenseSoftware.DataModels.DataModels;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.CalcDataModels
{
    public sealed class CalcDataModelLicKeyConcat: CalcDataModel
    {
        private static CalcDataModelLicKeyConcat _dataModel;

        private const string TableName = "LicKeyConcat";

        private CalcDataModelLicKeyConcat()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("ID LicenseKey").DataType = typeof(int);
            table.Columns.Add("ID License").DataType = typeof(int);
            table.Columns.Add("LicKey").DataType = typeof(string);
            table.PrimaryKey = new[] { table.Columns["ID LicKey"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DmLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelLicensesConcat");
            var config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var licenses = DataModelHelper.FilterRows(SoftLicensesDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var licKeys = DataModelHelper.FilterRows(SoftLicKeysDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var result = from licensesRow in licenses
                         join licKeyRow in licKeys
                         on licensesRow.Field<int>("ID License") equals licKeyRow.Field<int>("ID License")
                         select new
                         {
                             id_lickey = licKeyRow.Field<int>("ID LicenseKey"),
                             id_license = licensesRow.Field<int>("ID License"),
                             lickey = 
                                string.Format("{0} (в/н лицензии: {1}; срок действия: №{2} от {3})",
                                licKeyRow.Field<string>("LicKey"),
                                licensesRow.Field<int>("ID License"), 
                                licensesRow.Field<string>("DocNumber") ?? "б/н",
                                licensesRow.Field<DateTime>("BuyLicenseDate").ToString("dd.MM.yyyy", CultureInfo.InvariantCulture) +
                                    (licensesRow.Field<DateTime?>("ExpireLicenseDate") == null ? " (бессрочно)" :
                                    " по " + licensesRow.Field<DateTime>("ExpireLicenseDate").ToString("dd.MM.yyyy", CultureInfo.InvariantCulture))
                                )
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_lickey, x.id_license, x.lickey);
            });
            table.EndLoadData();
            if (!DataSetManager.DataSet.Tables.Contains(TableName))
                DataSetManager.AddTable(table);
            else
                DataSetManager.DataSet.Merge(table);
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelLicKeyConcat GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelLicKeyConcat());
        }

        public static bool HasInstance()
        {
            return _dataModel != null;
        }
    }
}
