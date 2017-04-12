using System.Windows.Forms;

namespace DataModels.DataModels
{
    public sealed class DevicesDataModel : DataModel
    {
        private static DevicesDataModel dataModel = null;
        private static string selectQuery = "SELECT * FROM Devices WHERE [ID Device Type] = 1";
        private static string tableName = "Devices";

        private DevicesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, selectQuery, tableName)
        {
        }

        protected override void ConfigureTable()
        {
            Table.PrimaryKey = new[] { Table.Columns["ID Device"] };
        }

        public static DevicesDataModel GetInstance()
        {
            return GetInstance(null, 0);
        }

        public static DevicesDataModel GetInstance(ToolStripProgressBar progressBar, int incrementor)
        {         
            if (dataModel == null)
                dataModel = new DevicesDataModel(progressBar, incrementor);
            return dataModel;
        }
    }
}
