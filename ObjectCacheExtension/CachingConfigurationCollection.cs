using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ObjectCacheExtension
{
    public class CachingConfigurationCollection : ConfigurationElementCollection
    {

        private static PolicyConfigurationElement defaultWhenNothingIsDefined = 
            new PolicyConfigurationElement{ Name="_internalDefault", IsDefault= true, Type = CachePolicyType.Sliding, Lifetime = 30};

        public PolicyConfigurationElement Default
        {
            get
            {
                PolicyConfigurationElement first = null;
                foreach (var k in BaseGetAllKeys())
                {
                    PolicyConfigurationElement p = BaseGet(k) as PolicyConfigurationElement;
                    if (first == null)
                        first = p;

                    if (p.IsDefault)
                        return p;
                }

                return first ?? defaultWhenNothingIsDefined;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new PolicyConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PolicyConfigurationElement)element).Name;
        }

        new public PolicyConfigurationElement this[string Name]
        {
            get
            {
                var policy = (PolicyConfigurationElement)BaseGet(Name);

                // Let the user know why they're getting a runtime error
                if(policy == null) 
                    throw new ArgumentException(string.Format("'{0}' is not defined as a caching policy in configuration", Name));

                return policy;
            }
        }

    }    
}
