using System;
using System.Configuration;

namespace ObjectCacheExtension
{
    public class CachingConfigurationCollection : ConfigurationElementCollection
    {
        private static PolicyConfigurationElement defaultWhenNothingIsDefined = new PolicyConfigurationElement
        {
            Name=  "_internalDefault",
            IsDefault = true, 
            Type = CachePolicyType.Sliding, 
            Lifetime = 30
        };

        public PolicyConfigurationElement Default
        {
            get
            {
                PolicyConfigurationElement first = null;
                foreach (var k in BaseGetAllKeys())
                {
                    PolicyConfigurationElement p = (PolicyConfigurationElement)BaseGet(k);
                    if (first == null)
                        first = p;

                    if (p != null && p.IsDefault)
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

        new public PolicyConfigurationElement this[string name]
        {
            get
            {
                var policy = (PolicyConfigurationElement)BaseGet(name);
                // returning default if an element by name wasn't found
                return policy ?? Default;
            }
        }

    }    
}
