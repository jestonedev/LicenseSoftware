using System;
using System.Data;
using System.Globalization;
using System.Linq;
using LicenseSoftware.DataModels.DataModels;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.CalcDataModels
{
    public sealed class CalcDataModelLicensesConcat: CalcDataModel
    {
        private static CalcDataModelLicensesConcat _dataModel;

        private const string TableName = "LicensesConcat";

        private CalcDataModelLicensesConcat()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("ID License").DataType = typeof(int);
            table.Columns.Add("ID Version").DataType = typeof(int);
            table.Columns.Add("ID Department").DataType = typeof(int);
            table.Columns.Add("License").DataType = typeof(string);
            table.PrimaryKey = new[] { table.Columns["ID License"] };
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
            // Вычисляем агрегационную информацию
            var result = from licensesRow in licenses
                         select new
                         {
                             id_license = licensesRow.Field<int>("ID License"),
                             id_version = licensesRow.Field<int>("ID Version"),
                             id_department = licensesRow.Field<int>("ID Department"),
                             license = "Документ: " + licensesRow.Field<string>("DocNumber") + "; Срок действия с " +
                                    licensesRow.Field<DateTime>("BuyLicenseDate").ToString("dd.MM.yyyy",CultureInfo.InvariantCulture) +
                                    (licensesRow.Field<DateTime?>("ExpireLicenseDate") == null ? " (бессрочно)" :
                                    " по " + licensesRow.Field<DateTime>("ExpireLicenseDate").ToString("dd.MM.yyyy", CultureInfo.InvariantCulture))
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_license, x.id_version, x.id_department, x.license);
            });
            table.EndLoadData();
            if (!DataSetManager.DataSet.Tables.Contains(TableName))
                DataSetManager.AddTable(table);
            else
                DataSetManager.DataSet.Merge(table);
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelLicensesConcat GetInstance()
        {
            return _dataModel ?? (_dataModel = new CalcDataModelLicensesConcat());
        }

        public static bool HasInstance()
        {
            return _dataModel != null;
        }
    }
}
