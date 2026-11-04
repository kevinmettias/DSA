namespace DSAExperimentation.LeetCode.Solutions.PeakIndexInAMountainArray;

// LC 852's promise about its input: the array is a mountain: strictly up to one peak inside it,
// then strictly down. LeetCode's judge never poses an input that breaks it. An array with several
// peaks has no single answer, and the scan and the search each find a different one. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class PeakIndexInAMountainArrayPromise
{
    public static bool IsKeptBy(int[] mountain)
    {
        var peak = 0;

        while (peak + 1 < mountain.Length && mountain[peak] < mountain[peak + 1])
        {
            peak++;
        }

        var end = peak;

        while (end + 1 < mountain.Length && mountain[end] > mountain[end + 1])
        {
            end++;
        }

        return end == mountain.Length - 1 && IsInside(peak, mountain.Length);
    }

    private static bool IsInside(int peak, int length) => peak > 0 && peak < length - 1;
}
