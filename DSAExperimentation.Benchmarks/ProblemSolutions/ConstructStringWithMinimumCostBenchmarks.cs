using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.AhoCorasick;
using DSAExperimentation.LeetCode.ConstructStringWithMinimumCost;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ConstructStringWithMinimumCostSolution's, the
// same methods ConstructStringWithMinimumCostTests proves correct. The
// automaton arm is handed its hoisted overload's prepared input - an already
// -built AhoCorasick plus the deduplicated word/cost arrays it and the DP walk
// over - so building the automaton is charged to [GlobalSetup] rather than to
// the construction being measured; the brute-force arm reads straight off the
// LeetCode-shaped words/costs since it never builds anything upfront.
[MemoryDiagnoser]
public class ConstructStringWithMinimumCostBenchmarks
{
    private const int Seed = 3213;
    private const string Alphabet = "abc";

    // A small, overlapping word set - single letters, common pairs, one triple -
    // so most target positions have several candidate matches of different
    // lengths, exercising the automaton's shared-prefix structure.
    private static readonly string[] Words = ["a", "b", "c", "ab", "bc", "ca", "abc"];
    private static readonly int[] Costs = [5, 5, 5, 3, 3, 3, 1];

    [Params(1_000, 20_000)]
    public int TargetLength;

    private string _target = null!;
    private List<string> _uniqueWords = null!;
    private int[] _uniqueCosts = null!;
    private AhoCorasick _automaton = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var chars = new char[TargetLength];

        for (var i = 0; i < TargetLength; i++)
        {
            chars[i] = Alphabet[random.Next(Alphabet.Length)];
        }

        _target = new string(chars);
        _uniqueWords = new List<string>(Words);
        _uniqueCosts = (int[])Costs.Clone();
        _automaton = new AhoCorasick(_uniqueWords);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceDp() => ConstructStringWithMinimumCostSolution.MinCostByBruteForceDp(_target, Words, Costs);

    [Benchmark]
    public int AhoCorasickDp() =>
        ConstructStringWithMinimumCostSolution.MinCostByAhoCorasickDp(_target, _automaton, _uniqueWords, _uniqueCosts);
}
