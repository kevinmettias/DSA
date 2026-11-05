using DSAExperimentation.Mechanisms.Cache;

namespace DSAExperimentation.LeetCode.Solutions.LRUCache;

// LeetCode's two calls on an LRU cache, which ICache answers under its own names (ARCHITECTURE 17.13): get
// answers -1 for a key the cache does not hold, where ICache's TryGetValue says whether it holds it, and put
// is ICache's Set.
internal static class LRUCacheCalls
{
    // LeetCode's answer to get for a key the cache does not hold.
    private const int Missing = -1;

    public static int Get(ICache<int, int> cache, int key)
    {
        if (!cache.TryGetValue(key, out var value))
        {
            return Missing;
        }

        return value;
    }

    public static void Put(ICache<int, int> cache, int key, int value) => cache.Set(key, value);
}
