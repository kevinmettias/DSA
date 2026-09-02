using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Iterator for Combination (LC 1286): generating every combination the
// judge's next()/hasNext() calls will eventually walk. BitmaskEnumeration is
// the naive approach - scan every one of the 2^n subset masks, keep the ones
// with popcount == combinationLength, then sort the results into
// lexicographic order. BacktrackComposed is this repo's Backtrack.Search
// (IteratorForCombinationTests precedent) walking indices in strictly
// increasing order, which is already lexicographic - no sort needed, and no
// wasted work on masks with the wrong popcount.
[MemoryDiagnoser]
public class IteratorForCombinationBenchmarks
{
    // The combination length is always half the character count for this benchmark's
    // fixture (see Setup).
    private const int CombinationLengthDivisor = 2;

    [Params(10, 16)]
    public int CharacterCount;

    private string _characters = null!;
    private int _combinationLength;

    [GlobalSetup]
    public void Setup()
    {
        _characters = new string(Enumerable.Range(0, CharacterCount).Select(i => (char)('a' + i)).ToArray());
        _combinationLength = CharacterCount / CombinationLengthDivisor;
    }

    [Benchmark(Baseline = true)]
    public List<string> BitmaskEnumeration()
    {
        var results = new List<string>();
        var subsetCount = 1 << _characters.Length;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            var combination = BuildCombinationIfMatch(mask);
            if (combination is not null)
            {
                results.Add(combination);
            }
        }

        results.Sort(StringComparer.Ordinal);
        return results;
    }

    private string? BuildCombinationIfMatch(int mask)
    {
        if (int.PopCount(mask) != _combinationLength)
        {
            return null;
        }

        var chars = new char[_combinationLength];
        var next = 0;

        for (var bit = 0; bit < _characters.Length; bit++)
        {
            if ((mask & (1 << bit)) != 0)
            {
                chars[next++] = _characters[bit];
            }
        }

        return new string(chars);
    }

    [Benchmark]
    public List<string> BacktrackComposed()
    {
        var results = new List<string>();
        var state = new State();

        Backtrack.Search<State, int>(
            state,
            isSolution: IsCombinationComplete,
            candidates: CandidateIndices,
            choose: Choose,
            unchoose: Unchoose,
            onSolution: x => results.Add(new string([.. x.Chosen])));

        return results;
    }

    private bool IsCombinationComplete(State x) => x.Chosen.Count == _combinationLength;

    private IEnumerable<int> CandidateIndices(State x)
        => x.Chosen.Count == _combinationLength
            ? []
            : Enumerable.Range(x.Start, _characters.Length - x.Start);

    private void Choose(State x, int index)
    {
        x.Starts.Push(x.Start);
        x.Chosen.Add(_characters[index]);
        x.Start = index + 1;
    }

    private static void Unchoose(State x, int _)
    {
        x.Start = x.Starts.Pop();
        x.Chosen.RemoveAt(x.Chosen.Count - 1);
    }

    private sealed class State
    {
        public List<char> Chosen { get; } = [];
        public Stack<int> Starts { get; } = new();
        public int Start { get; set; }
    }
}
