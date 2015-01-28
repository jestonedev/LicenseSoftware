using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseSoftware.Entities
{
    public sealed class SoftLicType
    {
        public int? IdLicType { get; set; }
        public string LicType { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SoftLicType));
        }

        public bool Equals(SoftLicType other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(SoftLicType first, SoftLicType second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
                    return first.IdLicType == second.IdLicType &&
                        first.LicType == second.LicType;
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
