using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BackspaceStringCompare;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BackspaceStringCompareSolution's, the same methods
// BackspaceStringCompareTests proves correct - the BCL's own Stack<char> replaying
// each string's keystrokes vs. this repo's own DynamicArray-backed Stack<char>
// doing exactly the same push-on-letter/pop-on-'#' replay, the same "same
// algorithm, BCL structure vs. repo structure" contrast OpenTheLockBenchmarks
// already draws for LC 752. _s and _t are built from the same seed, so both arms
// are forced through their full replay of both strings instead of short-circuiting
// on an early character mismatch.
[MemoryDiagnoser]
public class BackspaceStringCompareBenchmarks
{
    private const int RandomSeed = 844; // LC problem number
    private const int BackspaceChanceDenominator = 5;
    private const int AlphabetSize = 26;

    private string _s = "";

    private string _t = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _s = BuildKeystrokes(Length, seed: RandomSeed);
        _t = BuildKeystrokes(Length, seed: RandomSeed);
    }

    // ~20% backspaces so the stack genuinely grows and shrinks instead of only ever
    // growing - the same "reachable, real work" shaping LockWorkloads already
    // documents for its own random benchmark inputs.
    private static string BuildKeystrokes(int length, int seed)
    {
        var random = new Random(seed);
        var characters = new char[length];

        for (var i = 0; i < length; i++)
        {
            var isBackspace = random.Next(BackspaceChanceDenominator) == 0;
            characters[i] = isBackspace
                ? '#'
                : RandomLetter(random);
        }

        return new string(characters);
    }

    private static char RandomLetter(Random random) => (char)('a' + random.Next(AlphabetSize));

    [Benchmark(Baseline = true)]
    public bool BclStack() => BackspaceStringCompareSolution.BackspaceCompareByBclStack(_s, _t);

    [Benchmark]
    public bool RepoStack() => BackspaceStringCompareSolution.BackspaceCompareByStackReplay(_s, _t);
}
