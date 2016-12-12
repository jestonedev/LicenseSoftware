using System;

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
        public string Description { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SoftInstallation));
        }

        public bool Equals(SoftInstallation other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftInstallation first, SoftInstallation second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdLicense == second.IdLicense &&
                   first.IdComputer == second.IdComputer &&
                   first.InstallationDate == second.InstallationDate &&
                   first.IdComputer == second.IdComputer &&
                   first.InstallationDate == second.InstallationDate &&
                   first.IdLicenseKey == second.IdLicenseKey &&
                   first.IdInstallator == second.IdInstallator &&
                   first.Description == second.Description;
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
