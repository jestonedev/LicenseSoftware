namespace LicenseSoftware.Entities
{
    public sealed class SoftLicKey
    {
        public int? IdLicenseKey { get; set; }
        public int? IdLicense { get; set; }
        public string LicKey { get; set; }


        public override bool Equals(object obj)
        {
            return this == obj as SoftLicKey;
        }

        public bool Equals(SoftLicKey other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftLicKey first, SoftLicKey second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdLicenseKey == second.IdLicenseKey && 
                   first.IdLicense == second.IdLicense &&
                   first.LicKey == second.LicKey;
        }

        public static bool operator !=(SoftLicKey first, SoftLicKey second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
