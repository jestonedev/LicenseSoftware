using LicenseSoftware.Entities;

namespace LicenseSoftware.DataModels.CalcDataModels
{
    public class CalcAsyncConfig
    {
        public EntityType Entity { get; set; }
        public int? IdObject { get; set; }

        public CalcAsyncConfig(EntityType entity, int? idObject)
        {
            Entity = entity;
            IdObject = idObject;
        }
    }
}
