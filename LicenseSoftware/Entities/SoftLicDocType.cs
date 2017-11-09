namespace LicenseSoftware.Entities
{
    public sealed class SoftLicDocType
    {
        public int? IdDocType { get; set; }
        public string DocType { get; set; }
        
        public override bool Equals(object obj)
        {
            return (this == (obj as SoftLicDocType));
        }

        public bool Equals(SoftLicDocType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftLicDocType first, SoftLicDocType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdDocType == second.IdDocType &&
                   first.DocType == second.DocType;
        }

        public static bool operator !=(SoftLicDocType first, SoftLicDocType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
