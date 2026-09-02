using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Candy (LC 135): repeated left-to-right relaxation passes (keep re-scanning
// and bumping any candy count that still violates a neighbor's rating
// constraint, until a full pass makes no change) vs. the O(n) two-pass
// slope-constraint approach (a forward pass enforcing "rises must get more
// candy than their left neighbor," then a backward pass enforcing the same
// for falls). No repo primitive applies - a pure O(n) scan over the ratings
// array, the same "no stronger reusable primitive over a bare array" shape
// already established for GasStation/JumpGame/TrappingRainWater. _ratings is
// strictly decreasing, the worst case for repeated relaxation: each pass can
// only propagate one extra unit of "must exceed my right neighbor" one
// position further left, forcing O(n) passes of O(n) each.
[MemoryDiagnoser]
public class CandyBenchmarks
{
    private const int SecondToLastIndexOffset = 2;

    [Params(200, 3_000)]
    public int Length;

    private int[] _ratings = null!;

    [GlobalSetup]
    public void Setup() => _ratings = Enumerable.Range(0, Length).Select(i => Length - i).ToArray();

    [Benchmark(Baseline = true)]
    public int RepeatedRelaxation()
    {
        var candies = new int[_ratings.Length];
        Array.Fill(candies, 1);

        RelaxUntilStable(candies);

        return SumCandies(candies);
    }

    [Benchmark]
    public int TwoPassSlopeConstraints()
    {
        var candies = new int[_ratings.Length];
        Array.Fill(candies, 1);

        ApplyForwardSlopePass(candies);
        ApplyBackwardSlopePass(candies);

        return SumCandies(candies);
    }

    private void RelaxUntilStable(int[] candies)
    {
        bool changed;

        do
        {
            changed = false;

            for (var i = 0; i < _ratings.Length; i++)
            {
                changed |= RelaxNeighbors(candies, i);
            }
        } while (changed);
    }

    private bool RelaxNeighbors(int[] candies, int i)
    {
        var changed = false;

        if (i > 0 && _ratings[i] > _ratings[i - 1] && candies[i] <= candies[i - 1])
        {
            candies[i] = candies[i - 1] + 1;
            changed = true;
        }

        if (i < _ratings.Length - 1 && _ratings[i] > _ratings[i + 1] && candies[i] <= candies[i + 1])
        {
            candies[i] = candies[i + 1] + 1;
            changed = true;
        }

        return changed;
    }

    private void ApplyForwardSlopePass(int[] candies)
    {
        for (var i = 1; i < _ratings.Length; i++)
        {
            if (_ratings[i] > _ratings[i - 1])
            {
                candies[i] = candies[i - 1] + 1;
            }
        }
    }

    private void ApplyBackwardSlopePass(int[] candies)
    {
        for (var i = _ratings.Length - SecondToLastIndexOffset; i >= 0; i--)
        {
            if (_ratings[i] > _ratings[i + 1])
            {
                candies[i] = Math.Max(candies[i], candies[i + 1] + 1);
            }
        }
    }

    private static int SumCandies(int[] candies)
    {
        var total = 0;

        for (var i = 0; i < candies.Length; i++)
        {
            total += candies[i];
        }

        return total;
    }
}
