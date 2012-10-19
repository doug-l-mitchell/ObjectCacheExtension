using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Runtime.Caching;

namespace ObjectCacheExtension
{
    public class PolicyConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("isDefault", DefaultValue = false, IsRequired = false)]
        public bool IsDefault
        {
            get { return (bool)this["isDefault"]; }
            set { this["isDefault"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = "Sliding", IsRequired = false)]
        public CachePolicyType Type
        {
            get { return (CachePolicyType)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("lifeInMinutes", DefaultValue="60", IsRequired = false)]
        public long Lifetime
        {
            get { return (long)this["lifeInMinutes"]; }
            set { this["lifeInMinutes"] = value; }
        }

        internal static Dictionary<CachePolicyType, Func<PolicyConfigurationElement, CacheItemPolicy>> _policyFactory 
                                    = new Dictionary<CachePolicyType, Func<PolicyConfigurationElement, CacheItemPolicy>>
        {
            { CachePolicyType.Sliding, c =>  new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(c.Lifetime) } },
            { CachePolicyType.Absolute, c => new CacheItemPolicy { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(c.Lifetime) } },
            { CachePolicyType.Infinite, c => new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration} }
        };

        public CacheItemPolicy Policy { get { return _policyFactory[Type](this); } }
    }
}
