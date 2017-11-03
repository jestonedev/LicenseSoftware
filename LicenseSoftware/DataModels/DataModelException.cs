using System;
using System.Runtime.Serialization;

namespace LicenseSoftware.DataModels
{
    [Serializable]
    public class DataModelException: Exception
    {
        public DataModelException(string message)
            : base(message)
        {
        }

        public DataModelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public DataModelException()
            : base()
        {
        }

        protected DataModelException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
