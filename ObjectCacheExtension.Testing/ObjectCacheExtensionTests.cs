using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using ObjectCacheExtension;

namespace ObjectCacheExtension.Testing
{
    public class ObjectCacheExtensionTests
    {
        [TestFixture]
        public class AddOrGetExisting
        {
            private ObjectCache _cache;

            [TestFixtureSetUp]
            public void Init()
            {
                _cache = MemoryCache.Default;
            }

            internal static int GetData(int multiplier)
            {
                return 23 * multiplier;
            }

            [Test]
            public void ShouldCacheItemWhenNoPolicyIsSpecified()
            {
                var result = _cache.AddOrGetExisting<int>("test", () => GetData(7));

                Assert.AreEqual(GetData(7), result);
            }

            [Test]
            public void ShouldReturnSameCachedResultForMultipleThreads()
            {
                
                int [] vals = new int[10];
                var key = "test";
                
                // Each task will produce a different data value - 
                // It is expected that only the first thread will invoke the GetData method
                // and place its results in the cache. All other threads should retreive
                // the cached item 
                Parallel.Invoke(
                    () => { vals[0] = _cache.AddOrGetExisting<int>(key, () => GetData(1)); },
                    () => { vals[1] = _cache.AddOrGetExisting<int>(key, () => GetData(2)); },
                    () => { vals[2] = _cache.AddOrGetExisting<int>(key, () => GetData(3)); },
                    () => { vals[3] = _cache.AddOrGetExisting<int>(key, () => GetData(4)); },
                    () => { vals[4] = _cache.AddOrGetExisting<int>(key, () => GetData(5)); },
                    () => { vals[5] = _cache.AddOrGetExisting<int>(key, () => GetData(6)); },
                    () => { vals[6] = _cache.AddOrGetExisting<int>(key, () => GetData(7)); },
                    () => { vals[7] = _cache.AddOrGetExisting<int>(key, () => GetData(8)); },
                    () => { vals[8] = _cache.AddOrGetExisting<int>(key, () => GetData(9)); },
                    () => { vals[9] = _cache.AddOrGetExisting<int>(key, () => GetData(10)); }
                );

                // All values should be the same - it is indeterminate as to which 
                //task executed first so we can't say exactly what the value will be
                Assert.IsTrue(vals.Distinct().Count() == 1);
            }

            [Test]
            public void ShouldAddCacheItemWhenPolicyIsSpecified()
            {
                var firstResult = _cache.AddOrGetExisting<TestData>("testData", () => TestDataFactory.GetData(), CachingConfig.Policies["sixHours"].Policy);

                // can we get the item back out?
                var secondResult = _cache.AddOrGetExisting<TestData>("testData", () => { throw new Exception("I shouldn't have been called"); }, CachingConfig.Policies["sixHours"].Policy);
                Assert.AreSame(firstResult, secondResult);
            }

            [Test]
            public void ShouldAddCacheItemWhenPolicyNameIsSpecified()
            {
                var firstResult = _cache.AddOrGetExisting<TestData>("testData2", () => TestDataFactory.GetData(), "sixHours");

                // can we get the item back out?
                var secondResult = _cache.AddOrGetExisting<TestData>("testData2", () => { throw new Exception("I shouldn't have been called"); }, "sixHours");
                Assert.AreSame(firstResult, secondResult);
            }

            [Test]
            public void ShouldNotCacheWhenResultIsNull()
            {
                var key = "testNull";

                var result = _cache.AddOrGetExisting<TestData>(key, () => null);

                Assert.IsNull(result);

                result = _cache.AddOrGetExisting<TestData>(key, () => TestDataFactory.GetData());

                Assert.IsNotNull(result);
            }
        }
    }
}
