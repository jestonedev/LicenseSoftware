using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LicenseSoftware.Viewport
{
    public static class ViewportFactory
    {
        public static Viewport CreateViewport(IMenuCallback menuCallback, ViewportType viewportType)
        {
            switch (viewportType)
            {
                case ViewportType.SoftwareViewport:
                    return new SoftwareViewport(menuCallback);
                case ViewportType.LicensesViewport:
                    return new LicensesViewport(menuCallback);
                case ViewportType.InstallationsViewport:
                    break;
                case ViewportType.LicenseKeysViewport:
                    return new SoftLicKeysViewport(menuCallback);
                case ViewportType.InstallatorsViewport:
                    return new InstallatorsViewport(menuCallback);
                case ViewportType.SoftLicDocTypesViewport:
                    return new SoftLicDocTypesViewport(menuCallback);
                case ViewportType.SoftLicTypesViewport:
                    return new SoftLicTypesViewport(menuCallback);
                case ViewportType.SoftMakersViewport:
                    return new SoftMakersViewport(menuCallback);
                case ViewportType.SoftTypesViewport:
                    return new SoftTypesViewport(menuCallback);
                case ViewportType.SoftSuppliersViewport:
                    return new SoftSuppliersViewport(menuCallback);
            }
            throw new ViewportException(
                String.Format(CultureInfo.InvariantCulture, "В фабрику ViewportFactory передан неизвестный тип {0}", viewportType.ToString()));
        }
    }
}
