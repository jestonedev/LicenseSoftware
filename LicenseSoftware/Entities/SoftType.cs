namespace LicenseSoftware.Entities
{
    public sealed class SoftType
    {
        public int? IdSoftType { get; set; }
        public string SoftTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SoftType));
        }

        public bool Equals(SoftType other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftType first, SoftType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdSoftType == second.IdSoftType &&
                   first.SoftTypeName == second.SoftTypeName;
        }

        public static bool operator !=(SoftType first, SoftType second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
