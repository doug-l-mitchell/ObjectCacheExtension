ObjectCacheExtension
====================

Wrap data retrieval method calls with caching as easy as this:

```csharp
var data = cache.AddOrGetExisting<MyData>("mydata", () => repository.GetDataFromLongRunningProcess());
```

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
if(data == null)
{
	lock(syncroot) 
	{
		data = cache.Get("mydata");
		if(data == null)
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

Specifying cache eviction policies can be declared in the configuraiton file. You can let the extension choose the default policy, pass a policy in code, or specify a configured policy by name.

This is available on [NuGet](https://www.nuget.org/packages/ObjectCacheExtension/1.0.0.1)
------------------------------------------------------------------------------------------
