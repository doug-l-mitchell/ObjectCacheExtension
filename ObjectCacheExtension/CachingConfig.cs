using System;
using System.Configuration;

namespace ObjectCacheExtension
{
    /// <summary>
    /// Convenience method to access cache item policies
    /// </summary>
    public static class CachingConfig
    {
        private static readonly Lazy<CachingConfigurationCollection> _policies = new Lazy<CachingConfigurationCollection>(GetPolicies);

        private static CachingConfigurationCollection GetPolicies()
        {
            CachingConfigurationSection caching = ConfigurationManager.GetSection("caching") as CachingConfigurationSection;
            if (caching == null || caching.Policies == null)
            {
                throw new Exception("caching/policies path not found in config");
            }
            return caching.Policies;
        }

        public static CachingConfigurationCollection Policies
        {
            get { return _policies.Value; }
        }
    }
}
