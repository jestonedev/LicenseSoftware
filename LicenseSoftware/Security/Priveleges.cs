using System;

namespace Security
{
    [Flags]
    public enum Priveleges
    {
        None = 0,
        LicensesRead = 1,
        LicensesWrite = 2,
        LicensesReadWrite = 3,
        InstallationsRead = 4,
        InstallationsWrite = 8,
        InstallationsReadWrite = 12,
        DirectoriesRead = 16,
        DirectoriesWrite = 32,
        DirectoriesReadWrite = 48,
        AllPriveleges = LicensesReadWrite | InstallationsReadWrite | DirectoriesReadWrite
    }
}
