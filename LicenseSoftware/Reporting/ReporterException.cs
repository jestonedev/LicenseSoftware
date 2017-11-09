using System;
using System.Runtime.Serialization;

namespace LicenseSoftware.Reporting
{
    [Serializable]
    public class ReporterException : Exception
    {
        public ReporterException(string message)
            : base(message)
        {
        }

        public ReporterException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ReporterException()
        {
        }

        protected ReporterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
