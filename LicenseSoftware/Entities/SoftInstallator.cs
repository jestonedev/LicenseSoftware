using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseSoftware.Entities
{
    public sealed class SoftInstallator
    {
        public int? IdInstallator { get; set; }
        public string FullName { get; set; }
        public string Profession { get; set; }
        public bool? Inactive { get; set; } 

        public override bool Equals(object obj)
        {
            return (this == (obj as SoftInstallator));
        }

        public bool Equals(SoftInstallator other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(SoftInstallator first, SoftInstallator second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
            return first.IdInstallator == second.IdInstallator &&
                first.FullName == second.FullName &&
                first.Profession == second.Profession &&
                first.Inactive == second.Inactive;
        }

        public static bool operator !=(SoftInstallator first, SoftInstallator second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
