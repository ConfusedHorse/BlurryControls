using System;

namespace BlurryControls.Internals
{
    /// <summary>
    /// enum which enlists every viable system version
    /// </summary>
    [Obsolete("was used to determine blur strategy on windows, abandoned")]
    public enum OperatingSystem
    {
        Windows78,
        Windows10,
        NotSupported
    }
}