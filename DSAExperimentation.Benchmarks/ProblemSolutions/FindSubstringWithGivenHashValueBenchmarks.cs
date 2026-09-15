using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.FindSubstringWithGivenHashValue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindSubstringWithGivenHashValueSolution's, the same
// methods FindSubstringWithGivenHashValueTests proves correct - the O(n*k)
// window-rehash baseline against this repo's RollingHash queried in O(1) per
// window. The composed arm is handed the prefix table its hoisted overload takes,
// so the O(n) construction is charged to [GlobalSetup] rather than to the sweep
// being measured.
//
// UnreachableHashValue is deliberately outside [0, Modulo) so both strategies are
// forced through every window on every invocation instead of an early exit making
// the baseline look artificially competitive - the same
// deliberately-unreachable-target trick TwoSumBenchmarks uses.
[MemoryDiagnoser]
public class FindSubstringWithGivenHashValueBenchmarks
{
    private const int Power = 7;
    private const int Modulo = 1_000_000_007;
    private const int K = 20;
    private const long UnreachableHashValue = -1;
    private const int RandomSeed = 2156; // LC problem number
    private const int AlphabetSize = 26;

    private string _text = "";

    private RollingHash _reversedHash = null!;
    private static RollingHashLane Lane => new(Power, Modulo);

    [Params(500, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = (char)('a' + random.Next(AlphabetSize));
        }

        _text = new string(chars);
        _reversedHash = FindSubstringWithGivenHashValueSolution.BuildReversedWindowHash(_text, Lane);
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce() =>
        FindSubstringWithGivenHashValueSolution.TryFindSubstringByWindowRehash(
            _text, Lane, (WindowLength: K, HashValue: UnreachableHashValue), out _);

    [Benchmark]
    public bool RollingHashWindowed() =>
        FindSubstringWithGivenHashValueSolution.TryFindSubstringByRollingHash(
            _reversedHash, _text, (WindowLength: K, HashValue: UnreachableHashValue), out _);
}
