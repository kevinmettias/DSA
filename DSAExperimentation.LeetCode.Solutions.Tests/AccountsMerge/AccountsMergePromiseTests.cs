using DSAExperimentation.LeetCode.Solutions.AccountsMerge;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AccountsMerge;

public sealed partial class AccountsMergePromiseTests
{
    public static TheoryData<string[][], bool> Inputs =>
        new()
        {
            { [["John", "johnsmith@mail.com", "john_newyork@mail.com"], ["John", "johnsmith@mail.com", "john00@mail.com"], ["Mary", "mary@mail.com"]], true },
            { [["Alice", "alice1@example.com", "alice2@example.com"], ["Charlie", "charlie1@example.com", "alice2@example.com"]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenAccountsSharingAnEmailShareAName(string[][] accounts, bool expected)
        => Assert.Equal(expected, AccountsMergePromise.IsKeptBy(accounts));
}
