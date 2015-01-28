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
    public sealed class DepartmentsDataModel : DataModel
    {
        private static DepartmentsDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM Departments";
        private static string tableName = "Departments";

        private DepartmentsDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {   
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new DataColumn[] { Table.Columns["ID Department"] };
        }

        public static DepartmentsDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static DepartmentsDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new DepartmentsDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
