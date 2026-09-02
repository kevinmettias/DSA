using BenchmarkDotNet.Attributes;
using NestedStack = DSAExperimentation.DataStructures.Stack.Stack<DSAExperimentation.Benchmarks.ProblemSolutions.NestedInteger>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Flatten Nested List Iterator (LC 341): eager recursive flattening into a
// List<int> (the whole structure gets walked and materialized up front,
// regardless of how many elements a caller actually consumes) vs. this repo's
// own Stack<T> driving a lazy iterator that only unwraps a nested list once
// HasNext actually reaches it.
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
    public List<int> EagerRecursiveFlatten()
    {
        var flattened = new List<int>();
        FlattenInto(_nestedList, flattened);
        return flattened;
    }

    [Benchmark]
    public List<int> LazyStackIterator()
    {
        var iterator = new NestedIterator(_nestedList);
        var flattened = new List<int>();

        while (iterator.HasNext())
        {
            flattened.Add(iterator.Next());
        }

        return flattened;
    }

    private static void FlattenInto(List<NestedInteger> nestedList, List<int> destination)
    {
        foreach (var element in nestedList)
        {
            if (element.IsInteger)
            {
                destination.Add(element.Value);
            }
            else
            {
                FlattenInto(element.Elements, destination);
            }
        }
    }
}

// Mirrors LeetCode's own NestedInteger union type - see
// DSAExperimentation.Tests.LeetCodeCoverage.FlattenNestedListIterator.Fixtures.NestedInteger,
// duplicated here rather than shared since the Benchmarks project can't reference
// the Tests project's fixture types.
internal sealed class NestedInteger
{
    private readonly List<NestedInteger>? _list;
    private readonly int _value;

    private NestedInteger(int value, List<NestedInteger>? list)
    {
        _value = value;
        _list = list;
    }

    public bool IsInteger => _list is null;

    public int Value => IsInteger ? _value : throw new InvalidOperationException("Not an integer.");

    public List<NestedInteger> Elements => _list ?? throw new InvalidOperationException("Not a list.");

    public static NestedInteger OfInteger(int value) => new(value, null);

    public static NestedInteger OfList(params NestedInteger[] elements) => new(0, [.. elements]);
}

internal sealed class NestedIterator
{
    private readonly NestedStack _pending = new();

    public NestedIterator(List<NestedInteger> nestedList) => PushReversed(nestedList);

    public bool HasNext()
    {
        while (_pending.TryPeek(out var top) && !top.IsInteger)
        {
            _pending.TryPop(out _);
            PushReversed(top.Elements);
        }

        return _pending.Count > 0;
    }

    public int Next()
    {
        _pending.TryPop(out var top);
        return top.Value;
    }

    private void PushReversed(List<NestedInteger> elements)
    {
        for (var i = elements.Count - 1; i >= 0; i--)
        {
            _pending.Push(elements[i]);
        }
    }
}
