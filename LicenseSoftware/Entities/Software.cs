namespace LicenseSoftware.Entities
{
    public sealed class Software
    {
        public int? IdSoftware { get; set; }
        public int? IdSoftType { get; set; }
        public int? IdSoftMaker { get; set; }
        public string SoftwareName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as Software));
        }

        public bool Equals(Software other)
        {
            return Equals((object)other);
        }

        public static bool operator==(Software first, Software second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object) first == null || (object) second == null)
                return false;
            return first.IdSoftware == second.IdSoftware &&
                   first.IdSoftType == second.IdSoftType &&
                   first.IdSoftMaker == second.IdSoftMaker &&
                   first.SoftwareName == second.SoftwareName;
        }

        public static bool operator !=(Software first, Software second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
