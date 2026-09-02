using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Product of the Length of Two Palindromic Substrings (LC 1960): an O(n^3)
// baseline that, for every split point, re-derives the longest odd palindrome on each
// side from scratch via expand-around-every-center (O(n) centers * O(n) expansion,
// times O(n) split points) vs. this repo's own O(n) Manacher.ComputeOddRadii computed
// once, then an O(n^2)-worst-case pass over its per-center radii (still strictly less
// work than re-deriving every side from nothing) to build the same per-split left/right
// bests in a single forward and backward sweep. _text is all one repeated character, so
// every expansion in the baseline runs to the actual boundary instead of exiting after
// one comparison, forcing both strategies through genuinely large palindromes instead of
// an early exit on the first invocation making the naive version look artificially
// competitive.
[MemoryDiagnoser]
public class MaximumProductOfTheLengthOfTwoPalindromicSubstringsBenchmarks
{
    // Converts a Manacher-style radius count into the palindrome diameter it spans.
    private const int RadiusToDiameterMultiplier = 2;

    // The backward sweep starts one index before the last (already-seeded) slot.
    private const int BackwardSweepStartOffset = 2;

    [Params(30, 90)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup() => _text = new string('a', Length);

    [Benchmark(Baseline = true)]
    public long NaiveSplitScan()
    {
        var best = 0L;

        for (var split = 0; split < _text.Length - 1; split++)
        {
            var leftBest = LongestOddPalindromeInRange(0, split);
            var rightBest = LongestOddPalindromeInRange(split + 1, _text.Length - 1);
            best = Math.Max(best, (long)leftBest * rightBest);
        }

        return best;
    }

    private int LongestOddPalindromeInRange(int lo, int hi)
    {
        var best = 1;

        for (var center = lo; center <= hi; center++)
        {
            var radius = 0;

            while (center - radius - 1 >= lo && center + radius + 1 <= hi
                && _text[center - radius - 1] == _text[center + radius + 1])
            {
                radius++;
            }

            best = Math.Max(best, (RadiusToDiameterMultiplier * radius) + 1);
        }

        return best;
    }

    [Benchmark]
    public long ManacherSplitScan()
    {
        var oddRadii = Manacher.ComputeOddRadii(_text);
        var leftBest = BestPalindromeEndingAtOrBefore(_text.Length, oddRadii);
        var rightBest = BestPalindromeStartingAtOrAfter(_text.Length, oddRadii);

        var best = 0L;

        for (var split = 0; split < _text.Length - 1; split++)
        {
            best = Math.Max(best, (long)leftBest[split] * rightBest[split + 1]);
        }

        return best;
    }

    private static int[] BestPalindromeEndingAtOrBefore(int length, int[] oddRadii) =>
        BestPalindromeInDirection(length, oddRadii, forward: true);

    private static int[] BestPalindromeStartingAtOrAfter(int length, int[] oddRadii) =>
        BestPalindromeInDirection(length, oddRadii, forward: false);

    private static int[] BestPalindromeInDirection(int length, int[] oddRadii, bool forward)
    {
        var best = InitializeBest(length);
        FillFromRadii(best, oddRadii, forward);

        if (forward)
        {
            SweepForward(best);
        }
        else
        {
            SweepBackward(best);
        }

        return best;
    }

    private static int[] InitializeBest(int length)
    {
        var best = new int[length];
        Array.Fill(best, 1);
        return best;
    }

    private static void FillFromRadii(int[] best, int[] oddRadii, bool forward)
    {
        for (var center = 0; center < best.Length; center++)
        {
            for (var radius = 1; radius <= oddRadii[center]; radius++)
            {
                var edge = forward ? center + radius - 1 : center - radius + 1;
                best[edge] = Math.Max(best[edge], (RadiusToDiameterMultiplier * radius) - 1);
            }
        }
    }

    private static void SweepForward(int[] best)
    {
        for (var i = 1; i < best.Length; i++)
        {
            best[i] = Math.Max(best[i], best[i - 1]);
        }
    }

    private static void SweepBackward(int[] best)
    {
        for (var i = best.Length - BackwardSweepStartOffset; i >= 0; i--)
        {
            best[i] = Math.Max(best[i], best[i + 1]);
        }
    }
}
