using DSAExperimentation.LeetCode.InterleavingString;

namespace DSAExperimentation.LeetCode.Tests.InterleavingString;

// Harness only. Both arms are InterleavingStringSolution's competing strategies for
// one question - a memoized recursion over (i, j) offsets and a bottom-up rolling
// row - pinned to LeetCode's published examples.
public sealed partial class InterleavingStringSolutionTests
{
    public static TheoryData<InterleavingCase> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { new InterleavingCase(new InterleavingStrings("aabcc", "dbbca", "aadbbcbcac"), Expected: true) },
            { new InterleavingCase(new InterleavingStrings("aabcc", "dbbca", "aadbbbaccc"), Expected: false) },
            { new InterleavingCase(new InterleavingStrings("", "", ""), Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsInterleaveByMemoizedRecursion_LeetCodeExamples_ReturnsExpected(InterleavingCase example)
    {
        var actual = InterleavingStringSolution.IsInterleaveByMemoizedRecursion(
            example.Strings.First,
            example.Strings.Second,
            new InterleavingStringSolution.TargetText(example.Strings.Target));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsInterleaveByIterativeTable_LeetCodeExamples_ReturnsExpected(InterleavingCase example)
    {
        var actual = InterleavingStringSolution.IsInterleaveByIterativeTable(
            example.Strings.First,
            example.Strings.Second,
            new InterleavingStringSolution.TargetText(example.Strings.Target));

        Assert.Equal(example.Expected, actual);
    }

    // The three strings one interleaving question is asked about: the two inputs and the
    // target they must build. They travel together at every call site - both strategies
    // are called with exactly these - so they are one thing with a name rather than three
    // adjacent strings a caller can transpose.
    public readonly record struct InterleavingStrings(string First, string Second, string Target);

    // One LeetCode example: the strings to interleave, and whether they interleave. The
    // expectation is a named field of the row rather than a bare trailing argument, so a
    // data row says what it expects instead of leaving a reader to recall its position.
    public readonly record struct InterleavingCase(InterleavingStrings Strings, bool Expected);
}
