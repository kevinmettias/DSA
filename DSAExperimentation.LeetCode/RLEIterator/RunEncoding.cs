namespace DSAExperimentation.LeetCode.RLEIterator;

// The shape of LeetCode 900's flat encoding array: two ints per run pair, a count
// followed by the value it repeats, so run i's count sits at 2i and its value at
// 2i + 1. Separate from the solution because a benchmark that builds an encoding has
// to stride it by the same width the iterator reads it at.
internal static class RunEncoding
{
    public const int ValuesPerRun = 2;
}
