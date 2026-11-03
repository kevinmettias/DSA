namespace DSAExperimentation.LeetCode.Solutions.FindingMKAverage;

// The values LC 1825 fixes, declared once so FindingMKAverageSolution and its benchmark read the
// same copy instead of each restating it.
internal static class FindingMKAverageConstants
{
    // LC 1825 constrains every streamed value to 1 <= num <= 10^5, which is what
    // makes a value-indexed tree possible at all.
    internal const int MaxElementValue = 100_000;
}
