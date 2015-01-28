using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LicenseSoftware.Reporting
{
    public static class ReporterFactory
    {
        public static Reporter CreateReporter(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case ReporterType.EmptyReporter:
                    // Template
                    break;
            }
            throw new ReporterException(
                String.Format(CultureInfo.InvariantCulture, "В фабрику ReporterFactory передан неизвестный тип {0}", reporterType.ToString()));
        }
    }
}
