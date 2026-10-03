namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1122 - arr2 is every other integer from 0, and arr1
// holds each of arr2's values once, then fills its remaining slots with draws, half from
// arr2 (the ranked branch) and half from past arr2's range (the unranked, sort-by-value
// branch), and is shuffled. That keeps the guarantees LC 1122 states: every value inside
// [0, 1000], arr2's values distinct, and every one of them in arr1. The caller keeps
// referenceLength at most length and at most half of 1000, so arr2 and the range past it
// both fit.
internal static class RelativeSortArrayWorkloads
{
    private const int Arr2ValueStep = 2; // arr2 holds every other integer
    private const int FromReferenceOdds = 2; // random.Next(0, FromReferenceOdds) == 0 is a 50/50 draw
    private const int MaxArrayValue = 1_000; // LC 1122: 0 <= arr1[i], arr2[i] <= 1000

    public static (int[] Arr1, int[] Arr2) Build(int length, int referenceLength, int seed)
    {
        var random = new Random(seed);
        var arr2 = Enumerable.Range(0, referenceLength).Select(i => i * Arr2ValueStep).ToArray();
        var pastReferenceRange = referenceLength * Arr2ValueStep;
        var arr1 = new int[length];

        arr2.CopyTo(arr1, 0);

        for (var i = referenceLength; i < length; i++)
        {
            arr1[i] = IsFromReference(random)
                ? ReferenceValue(arr2, random)
                : random.Next(pastReferenceRange, MaxArrayValue + 1);
        }

        var order = SeededSequences.ShuffledZeroTo(length, random);

        return ([.. order.Select(slot => arr1[slot])], arr2);
    }

    private static bool IsFromReference(Random random) => random.Next(0, FromReferenceOdds) == 0;

    private static int ReferenceValue(int[] arr2, Random random) => arr2[random.Next(arr2.Length)];
}
