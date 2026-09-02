using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Assign Cookies (LC 455): the O(n*m) brute force (for every child, linearly
// rescan every remaining cookie for the smallest one that still satisfies it)
// vs. this repo's own O(n log n + m log m) MergeSort over ArrayIndexedSequence
// on both arrays, followed by a single O(n+m) two-pointer pass
// (AssignCookiesTests' algorithm). Both arrays are drawn from the same range so
// most children have several candidate cookies, forcing brute force through a
// long rescan per child instead of matching on the first cookie it looks at.
[MemoryDiagnoser]
public class AssignCookiesBenchmarks
{
    private const int RandomSeed = 455; // LC problem number
    private const int RandomValueUpperBound = 1_000;

    [Params(200, 3_000)]
    public int Length;

    private int[] _greed = null!;
    private int[] _sizes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _greed = Enumerable.Range(0, Length).Select(_ => random.Next(1, RandomValueUpperBound)).ToArray();
        _sizes = Enumerable.Range(0, Length).Select(_ => random.Next(1, RandomValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSmallestSufficientScan()
    {
        var sizes = (int[])_sizes.Clone();
        var used = new bool[sizes.Length];
        var content = 0;

        foreach (var greed in _greed)
        {
            if (TryAssignSmallestSufficientCookie(sizes, used, greed))
            {
                content++;
            }
        }

        return content;
    }

    private static bool TryAssignSmallestSufficientCookie(int[] sizes, bool[] used, int greed)
    {
        var bestIndex = FindSmallestSufficientCookieIndex(sizes, used, greed);

        if (bestIndex < 0)
        {
            return false;
        }

        used[bestIndex] = true;
        return true;
    }

    private static int FindSmallestSufficientCookieIndex(int[] sizes, bool[] used, int greed)
    {
        var bestIndex = -1;

        for (var j = 0; j < sizes.Length; j++)
        {
            if (!used[j] && sizes[j] >= greed && (bestIndex < 0 || sizes[j] < sizes[bestIndex]))
            {
                bestIndex = j;
            }
        }

        return bestIndex;
    }

    [Benchmark]
    public int SortThenTwoPointer()
    {
        var greed = (int[])_greed.Clone();
        var sizes = (int[])_sizes.Clone();

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(greed));
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sizes));

        var child = 0;
        var cookie = 0;

        while (child < greed.Length && cookie < sizes.Length)
        {
            if (sizes[cookie] >= greed[child])
            {
                child++;
            }

            cookie++;
        }

        return child;
    }
}
