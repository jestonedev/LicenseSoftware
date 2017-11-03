using LicenseSoftware.CalcDataModels;
using LicenseSoftware.Entities;
using System.Threading;
using Settings;

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

                        if (CalcDataModelLicensesConcat.HasInstance() && CalcDataModelLicensesConcat.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelLicensesConcat.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelLicensesConcat.GetInstance().DefferedUpdate = false;
                        }
                        if (CalcDataModelSoftwareConcat.HasInstance() && CalcDataModelSoftwareConcat.GetInstance().DefferedUpdate)
                        {
                            CalcDataModelSoftwareConcat.GetInstance().Refresh(EntityType.Unknown, null, true);
                            CalcDataModelSoftwareConcat.GetInstance().DefferedUpdate = false;
                        }
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
