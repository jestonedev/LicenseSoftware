using System.Globalization;

namespace LicenseSoftware.Reporting
{
    public static class ReporterFactory
    {
        public static Reporter CreateReporter(ReporterType reporterType)
        {
            switch (reporterType)
            {
                case ReporterType.LogInstallationsReporter:
                    return new LogInstallationsReporter();
                case ReporterType.LogLicensesReporter:
                    return new LogLicensesReporter();
                case ReporterType.LicensesBySoftCountReporter:
                    return new LicensesBySoftCountReporter();
                case ReporterType.InstallationsInfoReporter:
                    return new InstallationsInfoReporter();
                case ReporterType.DepartmentReporter:
                    return new DepartmentReporter();
                case ReporterType.PcReporter:
                    return new PcReporter();
            }
            throw new ReporterException(
                string.Format(CultureInfo.InvariantCulture, "В фабрику ReporterFactory передан неизвестный тип {0}", reporterType));
        }
    }
}
