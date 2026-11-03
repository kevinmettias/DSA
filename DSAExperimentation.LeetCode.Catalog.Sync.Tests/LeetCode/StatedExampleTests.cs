using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.LeetCode;

public sealed partial class StatedExampleTests
{
    // A stated input and the stored inputs, one per line, that it shows.
    public static TheoryData<string, string> Shown =>
        new()
        {
            { "nums = [2, 7, 11, 15], target = 9", "[2,7,11,15]\n9" },
            { "s = \"a,b\", k = 2", "\"a,b\"\n2" },
            { "[\"MinStack\", \"push\"] [[], [-2]]", "[\"MinStack\",\"push\"]\n[[],[-2]]" },
        };

    // A stated input and stored inputs it does not show: other values, and another number of them.
    public static TheoryData<string, string> NotShown =>
        new()
        {
            { "grid = [[2,4,3],[6,5,2]]", "[[3],[2]]" },
            { "nums = [3,3]", "[3,3]\n6" },
        };

    [Theory]
    [MemberData(nameof(Shown))]
    public void HasInputs_StatementShowingThem_IsTrue(string statedInput, string storedInputs)
    {
        var hasInputs = StatedExample.HasInputs(statedInput, storedInputs);

        Assert.True(hasInputs);
    }

    [Theory]
    [MemberData(nameof(NotShown))]
    public void HasInputs_StatementShowingOthers_IsFalse(string statedInput, string storedInputs)
    {
        var hasInputs = StatedExample.HasInputs(statedInput, storedInputs);

        Assert.False(hasInputs);
    }
}
