using System;

namespace NodeSystem.Runtime.Utils
{
    public static class GuidSystem
    {
        public static string NewGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}