namespace LicenseSoftware.Entities
{
    public sealed class SoftLicType
    {
        public int? IdLicType { get; set; }
        public string LicType { get; set; }
        public bool? LicKeyDuplicateAllowed { get; set; }

        public override bool Equals(object obj)
        {
            return this == obj as SoftLicType;
        }

        public bool Equals(SoftLicType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftLicType first, SoftLicType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdLicType == second.IdLicType &&
                   first.LicType == second.LicType &&
                   first.LicKeyDuplicateAllowed == second.LicKeyDuplicateAllowed;
        }

        public static bool operator !=(SoftLicType first, SoftLicType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
