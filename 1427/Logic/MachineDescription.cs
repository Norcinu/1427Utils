using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDTUtils
{
    /// <summary>
    /// Set these values at start up. Then use these for setting member variable values.
    /// </summary>
    static class MachineDescription
    {
        public static bool IsSpanish { get; set; }
        public static bool IsBritish { get; set; }

        public static int CountryCode { get; set; }
    }
}
