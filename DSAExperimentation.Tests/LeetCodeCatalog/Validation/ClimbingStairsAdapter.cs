using System.Text.Json;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Tests.LeetCodeCatalog.Validation;

internal readonly struct ClimbingStairsAdapter : ILeetCodeTestCaseAdapter<int, int>
{
    public int ParseInput(string rawInput) => JsonSerializer.Deserialize<int>(rawInput);

    public int ParseExpectedOutput(string rawExpectedOutput) => JsonSerializer.Deserialize<int>(rawExpectedOutput);

    public bool IsMatch(int actual, int expected) => LeetCodeAnswers.IsExactlyEqual(actual, expected);
}
