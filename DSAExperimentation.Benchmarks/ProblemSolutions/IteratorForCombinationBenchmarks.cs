using DSAExperimentation.LeetCode.IteratorForCombination;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are IteratorForCombinationSolution's, the same methods
// IteratorForCombinationSolutionTests proves correct. Each arm builds a fresh iterator - which
// is where every strategy does its work, since both precompute the whole combination
// list up front - and drains it, returning every combination Next produced in order. The
// alphabet is the first CharacterCount letters and the combination length is half of that,
// the widest point of the combination space.
public class IteratorForCombinationBenchmarks
{
    private const int CombinationLengthDivisor = 2;

    private string _characters = "";

    private int _combinationLength;

    private List<string> _combinations = [];
    [Params(10, 16)]
    public int CharacterCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _characters = new string(Enumerable.Range(0, CharacterCount).Select(i => (char)('a' + i)).ToArray());
        _combinationLength = CharacterCount / CombinationLengthDivisor;

        // Every combination is a subset of the alphabet, so there are never more than 2^n of them.
        _combinations = new List<string>(1 << CharacterCount);
    }

    [Benchmark(Baseline = true)]
    public List<string> BitmaskEnumeration()
    {
        var iterator = IteratorForCombinationSolution.CreateByBitmaskEnumeration(_characters, _combinationLength);
        return Drain(iterator);
    }

    [Benchmark]
    public List<string> BacktrackComposed()
    {
        var iterator = IteratorForCombinationSolution.CreateByBacktrackEngine(_characters, _combinationLength);
        return Drain(iterator);
    }

    private List<string> Drain(IteratorForCombinationSolution.CombinationIterator iterator)
    {
        _combinations.Clear();

        while (iterator.HasNext())
        {
            _combinations.Add(iterator.Next());
        }

        return _combinations;
    }
}
