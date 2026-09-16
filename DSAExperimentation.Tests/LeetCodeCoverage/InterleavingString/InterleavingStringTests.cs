using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InterleavingString;

public sealed partial class InterleavingStringTests
{
    public static TheoryData<InterleavingCase> Examples =>
        new()
        {
            { new InterleavingCase(new InterleavingStrings("aabcc", "dbbca", "aadbbcbcac"), Expected: true) },
            { new InterleavingCase(new InterleavingStrings("aabcc", "dbbca", "aadbbbaccc"), Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsInterleave_LeetCodeExamples_ReturnsExpected(InterleavingCase example)
    {
        var actual = IsInterleave(example.Strings);

        Assert.Equal(example.Expected, actual);
    }

    private static bool IsInterleave(InterleavingStrings strings)
    {
        if (strings.First.Length + strings.Second.Length != strings.Target.Length)
        {
            return false;
        }

        return Memoizer.Memoize<(int First, int Second), bool>((0, 0), new CanBuild(strings));
    }

    // The interleaving rule, named: the target is buildable when its next character
    // matches the next of either input and the rest stays buildable. The three strings
    // are what the decision is made against, so they arrive as the constructor's
    // parameter rather than as an ambient closure.
    private sealed class CanBuild(InterleavingStrings strings)
        : IRecurrence<(int First, int Second), bool>
    {
        public bool Replay((int First, int Second) state, IRecurrence<(int First, int Second), bool> rest)
        {
            var (i, j) = state; var k = i + j;
            return k == strings.Target.Length
                || (i < strings.First.Length && strings.First[i] == strings.Target[k] && rest.Replay((i + 1, j), rest))
                || (j < strings.Second.Length && strings.Second[j] == strings.Target[k] && rest.Replay((i, j + 1), rest));
        }
    }

    // The three strings one interleaving question is asked about: the two inputs and the
    // target they must build. They travel together at every call site - the recurrence
    // above is constructed from exactly these - so they are one thing with a name rather
    // than three adjacent strings a caller can transpose.
    public readonly record struct InterleavingStrings(string First, string Second, string Target);

    // One LeetCode example: the strings to interleave, and whether they interleave. The
    // expectation is a named field of the row rather than a bare trailing argument, so a
    // data row says what it expects instead of leaving a reader to recall its position.
    public readonly record struct InterleavingCase(InterleavingStrings Strings, bool Expected);
}
