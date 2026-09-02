using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Substring With Given Hash Value (LC 2156): the textbook O(n*k) baseline -
// recompute each length-k window's modular hash from scratch by walking its k
// characters - vs. this repo's RollingHash, built once in O(n) over the REVERSED
// text (see FindSubstringWithGivenHashValueTests' doc comment for why reversed) and
// then queried in O(1) per window. _unreachableHashValue is deliberately outside
// [0, Modulo) so both strategies are forced through every window on every
// invocation instead of an early exit making the baseline look artificially
// competitive - the same deliberately-unreachable-target trick TwoSumBenchmarks
// uses.
[MemoryDiagnoser]
public class FindSubstringWithGivenHashValueBenchmarks
{
    private const int Power = 7;
    private const int Modulo = 1_000_000_007;
    private const int K = 20;
    private const long UnreachableHashValue = -1;
    private const int RandomSeed = 2156; // LC problem number
    private const int AlphabetSize = 26;

    [Params(500, 20_000)]
    public int Length;

    private string _text = null!;
    private char[] _reversedChars = null!;

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
        _reversedChars = (char[])chars.Clone();
        Array.Reverse(_reversedChars);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var n = _text.Length;
        var found = -1;

        for (var i = 0; i <= n - K; i++)
        {
            if (WindowHash(i) == UnreachableHashValue)
            {
                found = i;
                break;
            }
        }

        return found;
    }

    private long WindowHash(int start)
    {
        long hash = 0;
        long powerTerm = 1;

        for (var j = 0; j < K; j++)
        {
            var value = _text[start + j] - 'a' + 1;
            hash = (hash + value * powerTerm) % Modulo;
            powerTerm = powerTerm * Power % Modulo;
        }

        return hash;
    }

    [Benchmark]
    public int RollingHashWindowed()
    {
        var comparer = EqualityComparer<char>.Create((left, right) => left == right, value => value - 'a' + 1);
        var lane = new RollingHashLane(Power, Modulo);
        var hash = new RollingHash(new string(_reversedChars), comparer, lane, lane);

        var n = _reversedChars.Length;
        var found = -1;

        for (var i = 0; i <= n - K; i++)
        {
            if (hash.Hash(n - i - K, K).First == UnreachableHashValue)
            {
                found = i;
                break;
            }
        }

        return found;
    }
}
