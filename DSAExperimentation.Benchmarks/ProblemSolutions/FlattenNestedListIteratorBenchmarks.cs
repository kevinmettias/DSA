using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FlattenNestedListIterator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FlattenNestedListIteratorSolution's, the same
// strategies FlattenNestedListIteratorTests proves correct. [GlobalSetup] builds
// the nested-list workload itself - already exactly the List<NestedInteger> shape
// both strategies' constructors take, so there is nothing further to hoist into a
// second overload.
[MemoryDiagnoser]
public class FlattenNestedListIteratorBenchmarks
{
    private const int LeavesPerNestedPair = 2;

    [Params(200, 5_000)]
    public int LeafCount;

    private List<NestedInteger> _nestedList = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Every leaf sits one level deep - [[0,1],[2,3],...] - so both strategies
        // do real unwrapping work for the whole input, not just a flat top level.
        _nestedList = [];

        for (var i = 0; i < LeafCount; i += LeavesPerNestedPair)
        {
            var pair = NestedInteger.OfList(NestedInteger.OfInteger(i), NestedInteger.OfInteger(i + 1));
            _nestedList.Add(pair);
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> EagerFlatten() => Drain(FlattenNestedListIteratorSolution.CreateByEagerFlatten(_nestedList));

    [Benchmark]
    public List<int> LazyStack() => Drain(FlattenNestedListIteratorSolution.CreateByLazyStack(_nestedList));

    private static List<int> Drain(FlattenNestedListIteratorSolution.IFlattenIterator iterator)
    {
        var flattened = new List<int>();

        while (iterator.HasNext())
        {
            flattened.Add(iterator.Next());
        }

        return flattened;
    }
}
