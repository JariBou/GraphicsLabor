using System;

namespace NodeSystem.Runtime.Core.PortConfigEnums
{
    [Flags]
    public enum FlowDirection
    {
        None = 0,
        Input = 1 << 0,
        Output = 1 << 1,
        Both = Input | Output
    }
}