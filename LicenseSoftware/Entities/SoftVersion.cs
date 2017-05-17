namespace LicenseSoftware.Entities
{
    public sealed class SoftVersion
    {
        public int? IdVersion { get; set; }
        public int? IdSoftware { get; set; }
        public string Version { get; set; }


        public override bool Equals(object obj)
        {
            return this == obj as SoftVersion;
        }

        public bool Equals(SoftVersion other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftVersion first, SoftVersion second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdVersion == second.IdVersion && 
                   first.IdSoftware == second.IdSoftware &&
                   first.Version == second.Version;
        }

        public static bool operator !=(SoftVersion first, SoftVersion second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
