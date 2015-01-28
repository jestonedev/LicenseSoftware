using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseSoftware.Entities
{
    public sealed class SoftInstallation
    {
        public int? IdInstallation { get; set; }
        public int? IdLicense { get; set; }
        public int? IdComputer { get; set; }
        public DateTime? InstallationDate { get; set; }
        public int? IdLicenseKey { get; set; }
        public int? IdInstallator { get; set; }
        public override bool Equals(object obj)
        {
            return (this == (obj as SoftInstallation));
        }

        public bool Equals(SoftInstallation other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(SoftInstallation first, SoftInstallation second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
                    return first.IdLicense == second.IdLicense &&
                first.IdComputer == second.IdComputer &&
                first.InstallationDate == second.InstallationDate &&
                first.IdComputer == second.IdComputer &&
                first.InstallationDate == second.InstallationDate &&
                first.IdLicenseKey == second.IdLicenseKey &&
                first.IdInstallator == second.IdInstallator;
        }

        public static bool operator !=(SoftInstallation first, SoftInstallation second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
