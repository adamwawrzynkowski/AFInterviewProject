using System;

namespace AFSInterview.Units
{
    [Flags]
    public enum UnitAttributes
    {
        Light       = 1,
        Armored     = 2,
        Mechanical  = 4,
        Fast        = 8
    }
}
