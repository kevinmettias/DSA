using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Boats to Save People (LC 881): repeatedly rescanning for the current heaviest and
// lightest remaining person (brute force, O(n^2)) vs. this repo's own MergeSort over
// ArrayIndexedSequence followed by a single O(n) two-pointer pass
// (BoatsToSavePeopleTests' algorithm) - AssignCookiesBenchmarks' shape, applied to a
// single array instead of two.
[MemoryDiagnoser]
public class BoatsToSavePeopleBenchmarks
{
    private const int Limit = 300;
    private const int RandomSeed = 881;

    [Params(200, 3_000)]
    public int Length;

    private int[] _people = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _people = Enumerable.Range(0, Length).Select(_ => random.Next(1, Limit)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRepeatedScan()
    {
        var weights = (int[])_people.Clone();
        var used = new bool[weights.Length];
        var remaining = weights.Length;
        var boats = 0;

        while (remaining > 0)
        {
            AssignOneBoat(weights, used, ref remaining);
            boats++;
        }

        return boats;
    }

    private void AssignOneBoat(int[] weights, bool[] used, ref int remaining)
    {
        var heaviestIndex = FindHeaviestUnused(weights, used);
        used[heaviestIndex] = true;
        remaining--;

        var lightestIndex = FindLightestUnusedFitting(weights, used, heaviestIndex);

        if (lightestIndex >= 0)
        {
            used[lightestIndex] = true;
            remaining--;
        }
    }

    private static int FindHeaviestUnused(int[] weights, bool[] used)
    {
        var heaviestIndex = -1;

        for (var i = 0; i < weights.Length; i++)
        {
            if (!used[i] && (heaviestIndex < 0 || weights[i] > weights[heaviestIndex]))
            {
                heaviestIndex = i;
            }
        }

        return heaviestIndex;
    }

    private static int FindLightestUnusedFitting(int[] weights, bool[] used, int heaviestIndex)
    {
        var lightestIndex = -1;

        for (var i = 0; i < weights.Length; i++)
        {
            if (!used[i] && weights[i] + weights[heaviestIndex] <= Limit
                && (lightestIndex < 0 || weights[i] < weights[lightestIndex]))
            {
                lightestIndex = i;
            }
        }

        return lightestIndex;
    }

    [Benchmark]
    public int SortThenTwoPointer()
    {
        var sorted = (int[])_people.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var light = 0;
        var heavy = sorted.Length - 1;
        var boats = 0;

        while (light <= heavy)
        {
            if (sorted[light] + sorted[heavy] <= Limit)
            {
                light++;
            }

            heavy--;
            boats++;
        }

        return boats;
    }
}
