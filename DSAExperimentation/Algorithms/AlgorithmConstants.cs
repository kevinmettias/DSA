namespace DSAExperimentation.Algorithms;

// Shared tuning knob for every bisection-based algorithm's midpoint arithmetic
// (BinarySearch, MergeSort) - not a shared Representation type, just the divisor
// each independently halves its range by.
internal static class AlgorithmConstants
{
    public const int HalvingFactor = 2;
}
