using DSAExperimentation.LeetCode.ValidAnagram;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidAnagram;

// Harness only. Both strategies are ValidAnagramSolution's - this file just pins
// them to LeetCode's published examples plus the differing-length case.
public sealed partial class ValidAnagramTests
{
    public static TheoryData<AnagramExample> Examples =>
        new()
        {
            { new AnagramExample(S: "anagram", T: "nagaram", Expected: true) },
            { new AnagramExample(S: "rat", T: "car", Expected: false) },
            { new AnagramExample(S: "aa", T: "a", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsAnagramByBruteForce_LeetCodeExamples_ReturnsWhetherTIsAnAnagramOfS(AnagramExample example)
    {
        var actual = ValidAnagramSolution.IsAnagramByBruteForce(example.S, example.T);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsAnagramByHashMapFrequencyCount_LeetCodeExamples_ReturnsWhetherTIsAnAnagramOfS(AnagramExample example)
    {
        var actual = ValidAnagramSolution.IsAnagramByHashMapFrequencyCount(example.S, example.T);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two strings to compare and whether the second is an
    // anagram of the first. `S` and `T` are the same `string` type and the question
    // is not symmetric, so the row names which is which rather than leaving two
    // interchangeable positions.
    public readonly record struct AnagramExample(string S, string T, bool Expected);
}
