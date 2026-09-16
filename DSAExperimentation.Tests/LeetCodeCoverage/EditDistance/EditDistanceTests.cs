using DSAExperimentation.LeetCode.EditDistance;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EditDistance;

// Harness only. EditDistanceSolution owns both the tabulated baseline and the
// memoized recurrence; this file pins them to LeetCode's published examples.
public sealed class EditDistanceTests
{
    public static TheoryData<EditDistanceCase> Examples =>
        new()
        {
            { new EditDistanceCase(Word1: "horse", Word2: "ros", Expected: 3) },
            { new EditDistanceCase(Word1: "intention", Word2: "execution", Expected: 5) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistanceByTabulation_LeetCodeExamples_ReturnsEditDistance(EditDistanceCase example)
    {
        var actual = EditDistanceSolution.MinDistanceByTabulation(example.Word1, example.Word2);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDistanceByMemoizedRecurrence_LeetCodeExamples_ReturnsEditDistance(EditDistanceCase example)
    {
        var actual = EditDistanceSolution.MinDistanceByMemoizedRecurrence(example.Word1, example.Word2);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two words and the edit distance between them. The words
    // are named fields rather than two adjacent `string` parameters, so a row is written
    // `new EditDistanceCase(Word1: ..., Word2: ...)` and a word1/word2 swap has to be
    // typed out by name instead of falling out of a position the compiler would have
    // accepted either way. Nested because it is only ever used inside this test class -
    // it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct EditDistanceCase(string Word1, string Word2, int Expected);
}
