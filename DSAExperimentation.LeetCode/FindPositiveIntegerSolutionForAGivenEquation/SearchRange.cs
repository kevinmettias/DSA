namespace DSAExperimentation.LeetCode.FindPositiveIntegerSolutionForAGivenEquation;

// LeetCode 1237's own search space: it pins both x and y to 1 <= x, y <= 1000, and
// every strategy's LeetCode-shaped overload is the one that walks exactly this
// square - the overloads taking an explicit bound exist so a benchmark can vary the
// search space without the range becoming part of the measured work. Separate from
// the solution because the tests that drive those LeetCode-shaped overloads declare
// their expectations against the same bound (2 * Bound + 1 is past the largest sum
// in range, Bound + Bound is exactly it).
internal static class SearchRange
{
    public const int Bound = 1000;
}
