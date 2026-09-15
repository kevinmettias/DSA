using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.IteratorForCombination;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IteratorForCombinationSolution's, the same methods
// IteratorForCombinationTests proves correct. Each arm builds a fresh iterator - which
// is where every strategy does its work, since both precompute the whole combination
// list up front - and drains it so the result cannot be elided, the same shape
// BinarySearchTreeIteratorBenchmarks uses (an iterator is internal, so a public
// [Benchmark] cannot return one; draining to an int both consumes it and keeps the
// arm a single call). The alphabet is the first CharacterCount letters and the
// combination length is half of that, the widest point of the combination space.
[MemoryDiagnoser]
public class IteratorForCombinationBenchmarks
{
    private const int CombinationLengthDivisor = 2;

    private string _characters = "";

    private int _combinationLength;
    [Params(10, 16)]
    public int CharacterCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _characters = new string(Enumerable.Range(0, CharacterCount).Select(i => (char)('a' + i)).ToArray());
        _combinationLength = CharacterCount / CombinationLengthDivisor;
    }

    [Benchmark(Baseline = true)]
    public int BitmaskEnumeration()
    {
        var iterator = IteratorForCombinationSolution.CreateByBitmaskEnumeration(_characters, _combinationLength);
        return Drain(iterator);
    }

    [Benchmark]
    public int BacktrackComposed()
    {
        var iterator = IteratorForCombinationSolution.CreateByBacktrackEngine(_characters, _combinationLength);
        return Drain(iterator);
    }

    private static int Drain(IteratorForCombinationSolution.CombinationIterator iterator)
    {
        var count = 0;

        while (iterator.HasNext())
        {
            iterator.Next();
            count++;
        }

        return count;
    }
}
