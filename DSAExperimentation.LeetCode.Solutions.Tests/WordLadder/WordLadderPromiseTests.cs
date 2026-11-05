using DSAExperimentation.LeetCode.Solutions.WordLadder;

namespace DSAExperimentation.LeetCode.Solutions.Tests.WordLadder;

public sealed partial class WordLadderPromiseTests
{
    public static TheoryData<string, string, string[], bool> Inputs =>
        new()
        {
            { "hit", "cog", ["hot", "dot", "dog", "lot", "log", "cog"], true },
            { "hit", "cog", ["hot", "hot"], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoWordRepeats(string begin, string end, string[] words, bool expected)
        => Assert.Equal(expected, WordLadderPromise.IsKeptBy(new(begin), new(end), words));
}
