using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Security
{
    [Flags]
    public enum Priveleges
    {
        NONE = 0,
        LICENSES_READ = 1,
        LICENSES_READ_WRITE = 3,
        INSTALLATIONS_READ = 4,
        INSTALLATIONS_READ_WRITE = 12,
        DIRECTORIES_READ = 16,
        DIRECTORIES_READ_WRITE = 48,
        ALL_PRIVELEGES = Priveleges.LICENSES_READ_WRITE | Priveleges.INSTALLATIONS_READ_WRITE | Priveleges.DIRECTORIES_READ_WRITE
    }
}
