using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return this.Equals((object)other);
        }

        public static bool operator ==(SoftType first, SoftType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
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
