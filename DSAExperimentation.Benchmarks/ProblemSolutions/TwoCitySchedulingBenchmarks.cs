using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Two City Scheduling (LC 1029): both strategies run the identical O(n log n)
// greedy - sort by (aCost - bCost) ascending, send the first half to city A - the
// same "same algorithm, different sort primitive" pairing
// QueueReconstructionByHeightBenchmarks already uses. ArraySortGreedy uses BCL
// Array.Sort; MergeSortGreedy instead composes this repo's own
// MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence.
[MemoryDiagnoser]
public class TwoCitySchedulingBenchmarks
{
    // Kept even: costs.Length must be 2n per LC 1029's own constraint.
    [Params(200, 4_000)]
    public int Length;

    private (int ACost, int BCost)[] _people = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1029);
        _people = Enumerable.Range(0, Length)
            .Select(_ => (ACost: random.Next(1, 1_000), BCost: random.Next(1, 1_000)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ArraySortGreedy()
    {
        var people = ((int ACost, int BCost)[])_people.Clone();
        Array.Sort(people, (a, b) => (a.ACost - a.BCost).CompareTo(b.ACost - b.BCost));

        var toCityA = people.Length / 2;
        var total = 0;

        for (var i = 0; i < people.Length; i++)
        {
            total += i < toCityA ? people[i].ACost : people[i].BCost;
        }

        return total;
    }

    [Benchmark]
    public int MergeSortGreedy()
    {
        var people = ((int ACost, int BCost)[])_people.Clone();
        var byCostDifferenceAscending = Comparer<(int ACost, int BCost)>.Create(
            (a, b) => (a.ACost - a.BCost).CompareTo(b.ACost - b.BCost));

        MergeSort.Sort<(int ACost, int BCost), ArrayIndexedSequence<(int ACost, int BCost)>>(
            new ArrayIndexedSequence<(int ACost, int BCost)>(people), byCostDifferenceAscending);

        var toCityA = people.Length / 2;
        var total = 0;

        for (var i = 0; i < people.Length; i++)
        {
            total += i < toCityA ? people[i].ACost : people[i].BCost;
        }

        return total;
    }
}
