using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Settings;

namespace LicenseSoftware.DataModels.DataModels
{
    public abstract class DataModel
    {
        private DataTable _table;
        private DataModelLoadState _dmLoadState = DataModelLoadState.BeforeLoad;
        private DataModelLoadSyncType _dmLoadType = DataModelLoadSyncType.Syncronize; // По умолчанию загрузка синхронная

        public DataModelLoadState DmLoadState { get { return _dmLoadState; } set { _dmLoadState = value; } }
        public DataModelLoadSyncType DmLoadType { get { return _dmLoadType; } set { _dmLoadType = value; } }
        protected DataTable Table { get { return _table; } set { _table = value; } }

        private static readonly object LockObj = new object();
        // Не больше MaxDBConnectionCount потоков одновременно делают запросы к БД
        private static readonly Semaphore DbAccessSemaphore = new Semaphore(LicenseSoftwareSettings.MaxDbConnectionCount,
            LicenseSoftwareSettings.MaxDbConnectionCount);

        protected DataModel()
        {
        }

        protected DataModel(ToolStripProgressBar progressBar, int incrementor, string selectQuery, string tableName)
        {
            var context = SynchronizationContext.Current;
            DmLoadType = DataModelLoadSyncType.Asyncronize;
            ThreadPool.QueueUserWorkItem((progress) =>
            {
                try
                {
                    DmLoadState = DataModelLoadState.Loading;
                    using (var connection = new DBConnection())
                    using (var command = DBConnection.CreateCommand())
                    {
                        command.CommandText = selectQuery;
                        DbAccessSemaphore.WaitOne();
                        Interlocked.Exchange(ref _table, connection.SqlSelectTable(tableName, command));
                    }
                    DbAccessSemaphore.Release();
                    ConfigureTable();
                    lock (LockObj)
                    {
                        DataSetManager.AddTable(Table);
                    }
                    DmLoadState = DataModelLoadState.SuccessLoad;
                    if (progress != null)
                    {
                        context.Post(_ => {
                            progressBar.Value += incrementor;
                            if (progressBar.Value == progressBar.Maximum)
                            {
                                progressBar.Visible = false;
                                //Если мы загрузили все данные, то запускаем CallbackUpdater
                                DataModelsCallbackUpdater.GetInstance().Run();
                                CalcDataModelsUpdater.GetInstance().Run();
                            }
                        }, null);
                    }
                }
                catch (SqlException e)
                {
                    lock (LockObj)
                    {
                        MessageBox.Show(String.Format(CultureInfo.InvariantCulture, 
                            "Произошла ошибка при загрузке данных из базы данных. Подробная ошибка: {0}", e.Message), "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        DmLoadState = DataModelLoadState.ErrorLoad;
                        Application.Exit();
                    }
                }
                catch (DataModelException e)
                {
                    MessageBox.Show(e.Message, "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    DmLoadState = DataModelLoadState.ErrorLoad;
                }
            }, progressBar); 
        }

        protected virtual void ConfigureTable()
        {
        }

        public virtual DataTable Select()
        {
            if (DmLoadType == DataModelLoadSyncType.Syncronize)
                return Table;
            while (DmLoadState != DataModelLoadState.SuccessLoad)
            {
                if (DmLoadState == DataModelLoadState.ErrorLoad)
                {
                    lock (LockObj)
                    {
                        MessageBox.Show("Произошла ошибка при загрузке данных из базы данных. Дальнейшая работа приложения невозможна", "Ошибка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        Application.Exit();
                        return null;
                    }
                }
                Application.DoEvents();
            }
            return Table;
        }
    }
}
