using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using LicenseSoftware.DataModels.DataModels;
using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.CalcDataModels
{
    public class CalcDataModel: DataModel, IDisposable
    {
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        public event EventHandler<EventArgs> RefreshEvent;
        
        // Метка отложенного обновления. Данные будут обновлены полностью при следующем проходе диспетчера обновления вычисляемых моделей
        public bool DefferedUpdate { get; set; }

        protected CalcDataModel()
        {
            DmLoadType = DataModelLoadSyncType.Asyncronize;
            _worker.DoWork += Calculate;
            _worker.RunWorkerCompleted += CalculationComplete;
        }

        public void Refresh(EntityType entity, int? idObject, bool removeDependenceEntities)
        {
            while (_worker.IsBusy)
                Application.DoEvents();
            if (removeDependenceEntities)
            {
                if (entity == EntityType.Unknown)
                    Table.Clear();
                else
                {
                    var removeRows = (from row in Table.AsEnumerable()
                                       where entity == EntityType.Software ? row.Field<int?>("ID Software") == idObject :
                                             entity == EntityType.SoftVersion ? row.Field<int?>("ID Version") == idObject :
                                             entity == EntityType.License ? row.Field<int?>("ID License") == idObject :
                                             entity == EntityType.Installation ? row.Field<int?>("ID Installation") == idObject : false
                                       select row);
                    for (var i = removeRows.Count() - 1; i >= 0; i--)
                        removeRows.ElementAt(i).Delete();
                }
            }
            _worker.RunWorkerAsync(new CalcAsyncConfig(entity, idObject));
        }

        protected virtual void Calculate(object sender, DoWorkEventArgs e)
        {
            // Здесь располагается асинхронно выполняющийся код вычисления модели
            // e.Argument - объект типа CalculationAsyncConfig, хранит информацию о сущности DataModelCalculateEnity и id объекта, который необходимо пересчитать
        }

        protected void CalculationComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // Делаем слияние результатов с текущей таблицей
            if (e == null)
                throw new DataModelException("Не передана ссылка на объект RunWorkerCompletedEventArgs в классе CalcDataModel");
            if (e.Error != null)
            {
                DmLoadState = DataModelLoadState.ErrorLoad;
                return;
            }
            if (e.Result is DataTable)
                Table.Merge((DataTable)e.Result);
            DmLoadState = DataModelLoadState.SuccessLoad;
            if (RefreshEvent != null)
                RefreshEvent(this, new EventArgs());
        }

        public void Dispose()
        {
            _worker.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
