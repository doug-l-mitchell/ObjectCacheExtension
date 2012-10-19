using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectCacheExtension
{
    [Serializable]
    public enum CachePolicyType
    {
        Sliding,
        Absolute,
        Infinite
    }
}
