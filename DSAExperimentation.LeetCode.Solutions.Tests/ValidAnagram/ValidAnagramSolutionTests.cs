using DSAExperimentation.LeetCode.Solutions.ValidAnagram;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ValidAnagram;

// Harness only. Both strategies are ValidAnagramSolution's - this file just pins
// them to the differing-length case.
public sealed partial class ValidAnagramSolutionTests
{
    public static TheoryData<AnagramCase> Cases =>
        new()
        {
            { new AnagramCase(S: "aa", T: "a", Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsAnagramByBruteForce_Cases_ReturnsWhetherTIsAnAnagramOfS(AnagramCase testCase)
    {
        var actual = ValidAnagramSolution.IsAnagramByBruteForce(testCase.S, testCase.T);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsAnagramByHashMapFrequencyCount_Cases_ReturnsWhetherTIsAnAnagramOfS(AnagramCase testCase)
    {
        var actual = ValidAnagramSolution.IsAnagramByHashMapFrequencyCount(testCase.S, testCase.T);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the two strings to compare and whether the second is an
    // anagram of the first. `S` and `T` are the same `string` type and the question
    // is not symmetric, so the row names which is which rather than leaving two
    // interchangeable positions.
    public readonly record struct AnagramCase(string S, string T, bool Expected);
}
