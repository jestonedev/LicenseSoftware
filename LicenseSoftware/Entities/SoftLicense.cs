using System;

namespace LicenseSoftware.Entities
{
    public sealed class SoftLicense
    {
        public int? IdLicense { get; set; }
        public int? IdVersion { get; set; }
        public int? IdLicType { get; set; }
        public int? IdDocType { get; set; }
        public int? IdSupplier { get; set; }
        public int? IdDepartment { get; set; }
        public string DocNumber { get; set; }
        public int? InstallationsCount { get; set; }
        public DateTime? BuyLicenseDate { get; set; }
        public DateTime? ExpireLicenseDate { get; set; }
        public string Description { get; set; }
        

        public override bool Equals(object obj)
        {
            return (this == obj as SoftLicense);
        }

        public bool Equals(SoftLicense other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftLicense first, SoftLicense second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdLicense == second.IdLicense &&
                   first.IdVersion == second.IdVersion &&
                   first.IdLicType == second.IdLicType &&
                   first.IdDocType == second.IdDocType &&
                   first.IdSupplier == second.IdSupplier &&
                   first.IdDepartment == second.IdDepartment &&
                   first.BuyLicenseDate == second.BuyLicenseDate &&
                   first.ExpireLicenseDate == second.ExpireLicenseDate &&
                   first.DocNumber == second.DocNumber &&
                   first.InstallationsCount == second.InstallationsCount &&
                   first.Description == second.Description;
        }

        public static bool operator !=(SoftLicense first, SoftLicense second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
