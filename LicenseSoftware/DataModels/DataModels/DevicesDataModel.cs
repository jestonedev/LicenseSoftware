using System.Windows.Forms;

namespace LicenseSoftware.DataModels.DataModels
{
    public sealed class DevicesDataModel : DataModel
    {
        private static DevicesDataModel _dataModel;
        private const string SelectQuery = "SELECT * FROM Devices WHERE [ID Device Type] = 1";
        private const string TableName = "Devices";

        private DevicesDataModel(ToolStripProgressBar progressBar, int incrementor)
            : base(progressBar, incrementor, SelectQuery, TableName)
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
            return _dataModel ?? (_dataModel = new DevicesDataModel(progressBar, incrementor));
        }
    }
}
