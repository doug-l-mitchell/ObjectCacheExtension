using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace ObjectCacheExtension
{
    /// <summary>
    /// This extension to ObjectCache provides for passing a function delegate that will retrieve the object that will
    /// be placed in the cache. Object retrieval will take some amount of time so we synchronize threads to ensure that
    /// the retrieval will happen only once.
    /// </summary>
    public static class ObjectCacheExtensions
    {
        /// <summary>
        /// Use the policy configured as the default
        /// </summary>
        public static T AddOrGetExisting<T>(this ObjectCache cacheProvider, string cacheKey, Func<T> fallbackFunction)
        {
            return AddOrGetExisting<T>(cacheProvider, cacheKey, fallbackFunction, CachingConfig.Policies.Default.Policy);
        }

        /// <summary>
        /// Specify the cache policy by configuration name
        /// </summary>
        public static T AddOrGetExisting<T>(this ObjectCache cacheProvider, string cacheKey, Func<T> fallbackFunction, string cachePolicyConfigurationName)
        {
            return AddOrGetExisting<T>(cacheProvider, cacheKey, fallbackFunction, CachingConfig.Policies[cachePolicyConfigurationName].Policy);
        }        

        /// <summary>
        /// This generic form of AddOrGetExisting is an improvement over the non-generic implementation in that this will return
        /// the value returned by <paramref name="fallbackFunction"/> if it doesn't already exist in the cache. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheProvider"></param>
        /// <param name="cacheKey"></param>
        /// <param name="fallbackFunction"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static T AddOrGetExisting<T>(this ObjectCache cacheProvider, string cacheKey, Func<T> fallbackFunction, CacheItemPolicy policy)
        {
            object objVal = cacheProvider.Get(cacheKey);
            if(objVal == null)
            {
                var lockObj = new object();
                // syncRoot will be null the first time through but is added to cache for the next requests
                var syncRoot = cacheProvider.AddOrGetExisting("syncroot_" + cacheKey, lockObj, CachingConfig.Policies["syncRootPolicy"].Policy);
                lock (syncRoot ?? lockObj)
                {
                    objVal = cacheProvider.Get(cacheKey);
                    if (objVal == null)
                    {
                        objVal = fallbackFunction.Invoke();

                        // Do not cache nulls
                        if (objVal != null)
                            cacheProvider.Set(cacheKey, objVal, policy);
                    }
                }
            }

            return (T)objVal;
        }

    }
}
