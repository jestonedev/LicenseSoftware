using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace LicenseSoftware.CalcDataModels
{
    public sealed class CalcDataModelLicensesConcat: CalcDataModel
    {
        private static CalcDataModelLicensesConcat dataModel = null;

        private static string tableName = "LicensesConcat";

        private CalcDataModelLicensesConcat()
            : base()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("ID License").DataType = typeof(int);
            table.Columns.Add("ID Software").DataType = typeof(int);
            table.Columns.Add("ID Department").DataType = typeof(int);
            table.Columns.Add("License").DataType = typeof(string);
            table.PrimaryKey = new DataColumn[] { table.Columns["ID License"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelLicensesConcat");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var licenses = DataModelHelper.FilterRows(SoftLicensesDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var result = from licenses_row in licenses
                         select new
                         {
                             id_license = licenses_row.Field<int>("ID License"),
                             id_software = licenses_row.Field<int>("ID Software"),
                             id_department = licenses_row.Field<int>("ID Department"),
                             license = "Документ: " + licenses_row.Field<string>("DocNumber") + "; Срок действия с " +
                                    licenses_row.Field<DateTime>("BuyLicenseDate").ToString("dd.MM.yyyy",CultureInfo.InvariantCulture) +
                                    (licenses_row.Field<DateTime?>("ExpireLicenseDate") == null ? " (бессрочно)" :
                                    " по " + licenses_row.Field<DateTime>("ExpireLicenseDate").ToString("dd.MM.yyyy", CultureInfo.InvariantCulture))
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_license, 
                    x.id_software, 
                    x.id_department,
                    x.license });
            });
            table.EndLoadData();
            if (!DataSetManager.DataSet.Tables.Contains(tableName))
                DataSetManager.AddTable(table);
            else
                DataSetManager.DataSet.Merge(table);
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelLicensesConcat GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelLicensesConcat();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
