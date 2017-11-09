namespace LicenseSoftware.Entities
{
    public sealed class SoftSupplier
    {
        public int? IdSoftSupplier { get; set; }
        public string SoftSupplierName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SoftSupplier));
        }

        public bool Equals(SoftSupplier other)
        {
            return Equals((object)other);
        }

        public static bool operator ==(SoftSupplier first, SoftSupplier second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            if ((object)first == null || (object)second == null)
                return false;
            return first.IdSoftSupplier == second.IdSoftSupplier &&
                   first.SoftSupplierName == second.SoftSupplierName;
        }

        public static bool operator !=(SoftSupplier first, SoftSupplier second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
