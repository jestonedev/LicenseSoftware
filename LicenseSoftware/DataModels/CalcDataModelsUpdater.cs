using LicenseSoftware.CalcDataModels;
using LicenseSoftware.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LicenseSoftware.DataModels
{      
    internal sealed class CalcDataModelsUpdater
    {
        private static CalcDataModelsUpdater instance;
        public void Run()
        {
            SynchronizationContext context = SynchronizationContext.Current;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    context.Send(__ =>
                    {
                        /* Макет
                        if (CalcDataModelBuildingsCurrentFunds.HasInstance() && CalcDataModelBuildingsCurrentFunds.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelBuildingsCurrentFunds.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelBuildingsCurrentFunds.GetInstance().DefferedUpdate = false;
                        }*/
                    }, null);
                    //Обновление делаем примерно каждые CalcDataModelsUpdateTimeout милисекунд
                    Thread.Sleep(LicenseSoftwareSettings.CalcDataModelsUpdateTimeout);
                }
            }, null);
        }

        public static CalcDataModelsUpdater GetInstance()
        {
            if (instance == null)
                instance = new CalcDataModelsUpdater();
            return instance;
        }
    }
}
