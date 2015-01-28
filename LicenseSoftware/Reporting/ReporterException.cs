using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            : base()
        {
        }

        protected ReporterException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
