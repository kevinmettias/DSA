using static DSAExperimentation.LeetCode.MapSumPairs.MapSumPairsSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MapSumPairs;

// Harness only. Both strategies are MapSumPairsSolution's - this file inserts
// every key/value in keys/values, then replays sum queries against each
// IMapSumStrategy implementation and checks the totals LeetCode itself
// publishes, so a failure still names the strategy that broke.
public sealed class MapSumPairsTests
{
    public static TheoryData<string[], int[], string[], int[]> Examples =>
        new()
        {
            // insert("apple", 3); sum("ap") == 3
            { ["apple"], [3], ["ap"], [3] },
            // insert("apple", 3); insert("app", 2); sum("ap") == 5
            { ["apple", "app"], [3, 2], ["ap"], [5] },
            // insert("apple", 3); insert("apple", 10) overrides; sum("apple") == 10
            { ["apple", "apple"], [3, 10], ["apple"], [10] },
            // insert("apple", 3); sum("banana") == 0 - unknown prefix
            { ["apple"], [3], ["banana"], [0] },
            // insert("apple", 3); insert("app", 2); insert("banana", 4);
            // sum("") == 9 - empty prefix sums every inserted value
            { ["apple", "app", "banana"], [3, 2, 4], [""], [9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MapSumByDictionaryScan_LeetCodeExamples_ReturnsExpectedSums(
        string[] keys, int[] values, string[] prefixes, int[] expected) =>
        AssertSums(new MapSumByDictionaryScan(), keys, values, prefixes, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void MapSumByTrieFold_LeetCodeExamples_ReturnsExpectedSums(
        string[] keys, int[] values, string[] prefixes, int[] expected) =>
        AssertSums(new MapSumByTrieFold(), keys, values, prefixes, expected);

    private static void AssertSums(
        IMapSumStrategy mapSum, string[] keys, int[] values, string[] prefixes, int[] expected)
    {
        for (var i = 0; i < keys.Length; i++)
        {
            mapSum.Insert(keys[i], values[i]);
        }

        for (var i = 0; i < prefixes.Length; i++)
        {
            Assert.Equal(expected[i], mapSum.Sum(prefixes[i]));
        }
    }
}
