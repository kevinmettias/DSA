using DSAExperimentation.LeetCode.Solutions.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SmallestKLengthSubsequenceWithOccurrencesOfALetter;

public sealed partial class SmallestKLengthSubsequenceWithOccurrencesOfALetterPromiseTests
{
    public static TheoryData<string, int, char, int, bool> Inputs =>
        new()
        {
            { "leet", 3, 'e', 1, true },
            { "leetcode", 4, 'e', 2, true },
            { "aabbccddeeff", 8, 'c', 3, false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenTheLetterIsThereEnough(string text, int subsequenceLength, char letter, int repetition, bool expected)
        => Assert.Equal(expected, SmallestKLengthSubsequenceWithOccurrencesOfALetterPromise.IsKeptBy(text, subsequenceLength, letter, repetition));
}
