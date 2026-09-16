using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LRUCacheBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - what LC 146's least-recently-used cache answers when the same
// call script is replayed against it - so a harness whose arms disagree is timing two different
// problems. Each arm creates its own cache inside the call and the script is a fixed list of
// stateless closures over whatever cache they are handed, so one harness instance is safe to call
// twice in either order; the arms cannot share eviction state through it.
//
// Each arm answers with the sum of every value the script's gets returned. That sum is an
// aggregate over the whole replay rather than the cache's final contents, so it would move if an
// eviction differed (a hit and a miss contribute different amounts), but it is not a witness that
// the two caches held the same entries at any point.
public sealed partial class LRUCacheBenchmarksTests
{
    private const int SmallestCapacity = 200;

    [Fact]
    public void Setup_SameCapacity_RebuildsTheSameCallScript() =>
        Assert.Equal(
            BuildHarness().DictionaryLinkedList(),
            BuildHarness().DictionaryLinkedList());

    [Fact]
    public void DictionaryLinkedList_SeededCallScript_AgreesWithLruCachePrimitive()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LruCachePrimitive(), harness.DictionaryLinkedList());
    }

    [Fact]
    public void LruCachePrimitive_SeededCallScript_AgreesWithDictionaryLinkedList()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryLinkedList(), harness.LruCachePrimitive());
    }

    private static LRUCacheBenchmarks BuildHarness()
    {
        var harness = new LRUCacheBenchmarks { Capacity = SmallestCapacity };
        harness.Setup();

        return harness;
    }
}
