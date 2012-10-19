using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace ObjectCacheExtension
{
    public class CachingConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("policies", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(CachingConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public CachingConfigurationCollection Policies
        {
            get
            {
                return this["policies"] as CachingConfigurationCollection;
            }
        }
    }
}
