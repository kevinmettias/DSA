using DSAExperimentation.DataStructures.Cache.LfuCache;
namespace DSAExperimentation.Tests.LeetCodeCoverage.LFUCache;
public sealed partial class LFUCacheTests { [Fact] public void LfuCache_Example_EvictsLeastFrequentlyUsed(){var cache=new LfuCache<int,int>(2);cache.Set(1,1);cache.Set(2,2);Assert.True(cache.TryGetValue(1,out var one));Assert.Equal(1,one);cache.Set(3,3);Assert.False(cache.TryGetValue(2,out _));Assert.True(cache.TryGetValue(3,out var three));Assert.Equal(3,three);} }
