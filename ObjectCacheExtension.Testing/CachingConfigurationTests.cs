using System;
using System.Runtime.Caching;

using NUnit.Framework;

namespace ObjectCacheExtension.Testing
{
    public class CachingConfigurationTests
    {
        [TestFixture]
        public class Collection
        {
            [Test]
            public void ShouldReturnDefaultWhenPolicyNameNotDefined()
            { 
                var policy = CachingConfig.Policies["MyCachingPolicy"];
                Assert.AreEqual(policy, CachingConfig.Policies.Default);
            }

            [Test]
            public void ShouldNotReturnNullForDefaultPolicy()
            {
                var policy = CachingConfig.Policies.Default;
                Assert.IsNotNull(policy);
            }

            [Test]
            public void ShouldGetValidPolicyWhenRequestingStandardSliding()
            {
                var policy = CachingConfig.Policies["StandardSliding"];

                Assert.IsNotNull(policy);
                Assert.AreEqual(10, policy.Lifetime);
                Assert.AreEqual(CachePolicyType.Sliding, policy.Type);
            }

            [Test]
            public void ShouldGetValidPolicyWHenRequestingSixHours()
            {
                var policy = CachingConfig.Policies["sixHours"];

                Assert.IsNotNull(policy);
                Assert.AreEqual(360, policy.Lifetime);
                Assert.AreEqual(CachePolicyType.Absolute, policy.Type);
            }
        }

        [TestFixture]
        public class PolicyConfigurationElementTests
        {
            [Test]
            public void ShouldReturnSlidingPolicyThatMatchesConfiguration()
            {
                PolicyConfigurationElement pce = new PolicyConfigurationElement { Name = "test", Type = CachePolicyType.Sliding, Lifetime = 13 };

                var policy = pce.Policy;

                Assert.AreEqual(13, policy.SlidingExpiration.Minutes);
                Assert.AreEqual(DateTime.MaxValue.ToFileTime(), policy.AbsoluteExpiration.ToFileTime());
            }

            [Test]
            public void ShouldReturnAbsolutePolicyThatMatchesConfiguration()
            {
                PolicyConfigurationElement pce = new PolicyConfigurationElement { Name = "test", Type = CachePolicyType.Absolute, Lifetime = 30 };
                var policy = pce.Policy;

                Assert.IsTrue((policy.AbsoluteExpiration - DateTime.Now).Minutes >= 29); 
            }

            [Test]
            public void ShouldReturnInfinitePolicyThatMatchesConfiguration()
            {
                PolicyConfigurationElement pce = new PolicyConfigurationElement { Type = CachePolicyType.Infinite };
                var policy = pce.Policy;

                Assert.AreEqual(ObjectCache.InfiniteAbsoluteExpiration, policy.AbsoluteExpiration);
            }
        }
    }
}
