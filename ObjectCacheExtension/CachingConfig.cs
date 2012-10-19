using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ObjectCacheExtension
{
    /// <summary>
    /// Convenience method to access cache item policies
    /// </summary>
    public static class CachingConfig
    {
        public static CachingConfigurationCollection Policies
        {
            get
            {
                CachingConfigurationSection caching = ConfigurationManager.GetSection("caching") as CachingConfigurationSection;
                if (caching == null || caching.Policies == null)
                    throw new Exception("Caching configuration was not found");

                return caching.Policies;
            }
        }
    }
}
