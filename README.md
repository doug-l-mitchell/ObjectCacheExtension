ObjectCacheExtension
====================

[![NuGet Badge](https://buildstats.info/nuget/ObjectCacheExtension)](https://www.nuget.org/packages/ObjectCacheExtension/) [![.NET Framework](https://img.shields.io/badge/.NET%20Framework-%3E%3D%204.0-red.svg)](#) [![.NET Standard](https://img.shields.io/badge/.NET%20Standard-%3E%3D%202.0-red.svg)](#)


Wrap data retrieval method calls with caching as easy as this:

```csharp
var data = cache.AddOrGetExisting<MyData>("mydata", () => repository.GetDataFromLongRunningProcess());
```

### Install ObjectCacheExtension via NuGet

If you want to include ObjectCacheExtension in your project, you can [install it directly from NuGet](https://www.nuget.org/packages/ObjectCacheExtension)

To install ObjectCacheExtension, run the following command in the Package Manager Console

```
PM> Install-Package ObjectCacheExtension
```

Then ensure that the `<caching>` section is implemented in your configuration file.

How is AddOrGetExisting&lt;T&gt; different?
-------------------------------------

ObjectCache defines three methods for placing objects in the cache: 

```csharp
bool Add(string key, object value, CacheItemPolicy policy, string regionName = null);
void Set(string key, object value, CacheItemPolicy policy, string regionName = null);
object AddOrGetExisting(string key, object value, CacheItemPolicy policy, string regionName = null);
```

Each of these have overrides but in each case you must already have the value object that is to be placed in the cache. This can lead to code like this:

```csharp
var data = cache.Get("mydata");
if (data == null)
{
    lock (syncroot)
    {
        data = cache.Get("mydata");
        if (data == null)
        {
            data = repository.GetDataFromLongRunningProcess();
            cache.Set("mydata", data, cachePolicy);
        }
    }
}
return data;
```

The cache is thread-safe in that it protects reads and writes but the method call to retrieve data needs to be protected as well. This extension manages synchronizing the function call to ensure that the cost of retrieving the data will happen only once when multiple threads are requesting data that isn't currently in cache.

CacheItemPolicy Configuration
-----------------------------

Specifying cache eviction policies can be declared in the configuration file by adding the `caching` section. You can let the extension choose the default policy, pass a policy in code, or specify a configured policy by name.

```xml
<configuration>
  <configSections>
    <section name="caching" type="ObjectCacheExtension.CachingConfigurationSection, ObjectCacheExtension" />
  </configSections>
  <caching>
    <policies>
      <add name="StandardSliding" isDefault="true" type="Sliding" lifeInMinutes="10"/>
      <!-- 
        syncRootPolicy is required by AddOrGetExisting<T> 
        A unique syncRoot is used for each cache key but these only need to live as long as
        data is being retrieved. The lifeInMinutes can be as long as you like but the syncRoot 
        only needs to be cached for the length of time it takes the fallbackFunction to 
        exectute. This one policy is used for all calls to AddOrGetExisting<T> so it should
        reflect the longest time possible.
      -->
      <add name="syncRootPolicy" type="Sliding" lifeInMinutes="3"/>
      <!-- Add additional cache policy definitions here
      <add name="Forever" type="Infinite" />
      <add name="Absolute" type="Absolute" lifeInMinutes="300" />
	  -->
    </policies>
  </caching>
</configuration>
```

Special thanks to Alexander Batishchev for improvements to this code.
