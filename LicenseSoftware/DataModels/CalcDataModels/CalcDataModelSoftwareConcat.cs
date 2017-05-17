using LicenseSoftware.DataModels;
using LicenseSoftware.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using DataModels.DataModels;

namespace LicenseSoftware.CalcDataModels
{
    public sealed class CalcDataModelSoftwareConcat: CalcDataModel
    {
        private static CalcDataModelSoftwareConcat dataModel;

        private const string TableName = "SoftwareConcat";

        private CalcDataModelSoftwareConcat()
        {
            Table = InitializeTable();
            Refresh(EntityType.Unknown, null, false);
        }

        private static DataTable InitializeTable()
        {
            var table = new DataTable(TableName) {Locale = CultureInfo.InvariantCulture};
            table.Columns.Add("ID Version").DataType = typeof(int);
            table.Columns.Add("Software").DataType = typeof(string);
            table.PrimaryKey = new[] { table.Columns["ID Version"] };
            return table;
        }

        protected override void Calculate(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            DMLoadState = DataModelLoadState.Loading;
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект DoWorkEventArgs в классе CalcDataModelSoftwareConcat");
            var config = (CalcAsyncConfig)e.Argument;
            // Фильтруем удаленные строки
            var software = DataModelHelper.FilterRows(SoftwareDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            var versions = DataModelHelper.FilterRows(SoftVersionsDataModel.GetInstance().Select(), config.Entity, config.IdObject);
            // Вычисляем агрегационную информацию
            var result = from softwareRow in software
                         join versionRow in versions
                         on softwareRow.Field<int>("ID Software") equals versionRow.Field<int>("ID Software")
                         select new
                         {
                             id_version = versionRow.Field<int>("ID Version"),
                             software = softwareRow.Field<string>("Software") +
                                    (versionRow.Field<string>("Version") == null ? "" : " " + versionRow.Field<string>("Version"))
                         };
            // Заполняем таблицу изменений
            var table = InitializeTable();
            table.BeginLoadData();
            result.ToList().ForEach(x =>
            {
                table.Rows.Add(x.id_version, x.software);
            });
            table.EndLoadData();
            if (!DataSetManager.DataSet.Tables.Contains(TableName))
                DataSetManager.AddTable(table);
            else
                DataSetManager.DataSet.Merge(table);
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
