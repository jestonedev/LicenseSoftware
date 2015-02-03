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
    public sealed class CalcDataModelSoftwareConcat: CalcDataModel
    {
        private static CalcDataModelSoftwareConcat dataModel = null;

        private static string tableName = "SoftwareConcat";

        private CalcDataModelSoftwareConcat()
            : base()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            DataTable table = new DataTable(tableName);
            table.Locale = CultureInfo.InvariantCulture;
            table.Columns.Add("ID Software").DataType = typeof(int);
            table.Columns.Add("Software").DataType = typeof(string);
            table.PrimaryKey = new DataColumn[] { table.Columns["ID Software"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelSoftwareConcat");
            CalcAsyncConfig config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var software = DataModelHelper.FilterRows(SoftwareDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var result = from software_row in software
                         select new
                         {
                             id_software = software_row.Field<int>("ID Software"),
                             software = software_row.Field<string>("Software") + 
                                    (software_row.Field<string>("Version") == null ? "" : " " + software_row.Field<string>("Version"))
                         };
            // Заполняем таблицу изменений
            DataTable table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach((x) =>
            {
                table.Rows.Add(new object[] { 
                    x.id_software, 
                    x.software });
            });
            table.EndLoadData();
            // Возвращаем результат
            e.Result = table;
        }

        public static CalcDataModelSoftwareConcat GetInstance()
        {
            if (dataModel == null)
                dataModel = new CalcDataModelSoftwareConcat();
            return dataModel;
        }

        public static bool HasInstance()
        {
            return dataModel != null;
        }
    }
}
