using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LicenseSoftware.Entities
{
    public sealed class SoftMaker
    {
        public int? IdSoftMaker { get; set; }
        public string SoftMakerName { get; set; }

        public override bool Equals(object obj)
        {
            return (this == (obj as SoftMaker));
        }

        public bool Equals(SoftMaker other)
        {
            return this.Equals((object)other);
        }

        public static bool operator ==(SoftMaker first, SoftMaker second)
        {
            if ((object)first == null && (object)second == null)
                return true;
            else
                if ((object)first == null || (object)second == null)
                    return false;
                else
                    return first.IdSoftMaker == second.IdSoftMaker &&
                        first.SoftMakerName == second.SoftMakerName;
        }

        public static bool operator !=(SoftMaker first, SoftMaker second)
        {
            return !(first == second);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
