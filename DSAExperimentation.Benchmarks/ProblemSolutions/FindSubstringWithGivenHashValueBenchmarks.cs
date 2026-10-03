using DSAExperimentation.DataStructures.RollingHash;
using DSAExperimentation.LeetCode.FindSubstringWithGivenHashValue;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindSubstringWithGivenHashValueSolution's, the same
// methods FindSubstringWithGivenHashValueSolutionTests proves correct - the O(n*k)
// window-rehash baseline against this repo's RollingHash queried in O(1) per
// window. The composed arm is handed the prefix table its hoisted overload takes,
// so the O(n) construction is charged to [GlobalSetup] rather than to the sweep
// being measured.
//
// LC 2156 guarantees an answer exists, so the target is planted: the hash of the
// text's LAST window, which no earlier window shares at either Length. Both
// strategies are therefore forced through every window on every invocation instead
// of an early exit making the baseline look artificially competitive. Modulo is LC
// 2156's largest, 10^9.
public class FindSubstringWithGivenHashValueBenchmarks
{
    private const int Power = 7;
    private const int Modulo = 1_000_000_000;
    private const int K = 20;
    private const int RandomSeed = 2156; // LC problem number
    private const int AlphabetSize = 26;

    private string _text = "";

    private long _lastWindowHash;

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
        _lastWindowHash = LastWindowHash(_text);
        _reversedHash = FindSubstringWithGivenHashValueSolution.BuildReversedWindowHash(_text, Lane);
    }

    // LC 2156's hash of the text's final K characters: the window's own first character
    // on power^0, each letter valued by its alphabet index plus one.
    private static long LastWindowHash(string text)
    {
        var hash = 0L;
        var powerTerm = 1L;

        foreach (var letter in text[^K..])
        {
            var letterValue = letter - 'a' + 1;
            hash = (hash + (letterValue * powerTerm)) % Modulo;
            powerTerm = powerTerm * Power % Modulo;
        }

        return hash;
    }

    [Benchmark(Baseline = true)]
    public bool TryFindSubstringByWindowRehash() =>
        FindSubstringWithGivenHashValueSolution.TryFindSubstringByWindowRehash(
            _text, Lane, (WindowLength: K, HashValue: _lastWindowHash), out _);

    [Benchmark]
    public bool TryFindSubstringByRollingHash() =>
        FindSubstringWithGivenHashValueSolution.TryFindSubstringByRollingHash(
            _reversedHash, _text, (WindowLength: K, HashValue: _lastWindowHash), out _);
}
