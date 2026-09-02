using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// The Number of Weak Characters in the Game (LC 1996): PairwiseComparison checks every
// character against every other character directly - O(n^2), the literal problem
// definition with no preprocessing at all - vs. SortThenScan, which sorts by attack
// descending / defense ascending via this repo's own MergeSort.Sort<Element,TSequence>
// and then makes one O(n) linear pass tracking the running maximum defense seen so far -
// the same pair of primitives TheNumberOfWeakCharactersInTheGameTests.cs exercises for
// correctness. _properties is deliberately generated with a small attack range so many
// duplicate attack values occur, exercising the descending-attack/ascending-defense
// tie-break rather than only the simple strictly-decreasing case.
[MemoryDiagnoser]
public class TheNumberOfWeakCharactersInTheGameBenchmarks
{
    private const int RandomSeed = 1996;
    private const int MaxAttackExclusive = 50;
    private const int MaxDefenseExclusive = 1_000;

    [Params(200, 3_000)]
    public int Length;

    private (int Attack, int Defense)[] _properties = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _properties = Enumerable.Range(0, Length)
            .Select(_ => (Attack: random.Next(1, MaxAttackExclusive), Defense: random.Next(1, MaxDefenseExclusive)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseComparison()
    {
        var weakCount = 0;

        for (var i = 0; i < _properties.Length; i++)
        {
            for (var j = 0; j < _properties.Length; j++)
            {
                if (i == j)
                {
                    continue;
                }

                if (_properties[j].Attack > _properties[i].Attack && _properties[j].Defense > _properties[i].Defense)
                {
                    weakCount++;
                    break;
                }
            }
        }

        return weakCount;
    }

    [Benchmark]
    public int SortThenScan()
    {
        var items = ((int Attack, int Defense)[])_properties.Clone();
        SortByAttackDescendingDefenseAscending(items);

        var weakCount = 0;
        var maxDefenseSoFar = 0;

        foreach (var (_, defense) in items)
        {
            if (defense < maxDefenseSoFar)
            {
                weakCount++;
            }

            maxDefenseSoFar = Math.Max(maxDefenseSoFar, defense);
        }

        return weakCount;
    }

    private static void SortByAttackDescendingDefenseAscending((int Attack, int Defense)[] items)
    {
        var byAttackDescendingDefenseAscending = Comparer<(int Attack, int Defense)>.Create(
            (a, b) => a.Attack != b.Attack ? b.Attack.CompareTo(a.Attack) : a.Defense.CompareTo(b.Defense));

        MergeSort.Sort<(int Attack, int Defense), ArrayIndexedSequence<(int Attack, int Defense)>>(
            new ArrayIndexedSequence<(int Attack, int Defense)>(items), byAttackDescendingDefenseAscending);
    }
}
