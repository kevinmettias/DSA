using DSAExperimentation.LeetCode.AccountsMerge;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AccountsMerge;

// Harness only. Both strategies are AccountsMergeSolution's - this file just pins
// them to LeetCode's published examples. Merged-account order is unspecified by
// LeetCode, so examples are checked as a set of expected accounts rather than by
// position.
public sealed partial class AccountsMergeTests
{
    public static TheoryData<string[][], string[][]> Examples =>
        new()
        {
            {
                new[]
                {
                    new[] { "John", "johnsmith@mail.com", "john_newyork@mail.com" },
                    new[] { "John", "johnsmith@mail.com", "john00@mail.com" },
                    new[] { "Mary", "mary@mail.com" },
                    new[] { "John", "johnnybravo@mail.com" },
                },
                new[]
                {
                    new[] { "John", "john00@mail.com", "john_newyork@mail.com", "johnsmith@mail.com" },
                    new[] { "Mary", "mary@mail.com" },
                    new[] { "John", "johnnybravo@mail.com" },
                }
            },
            {
                new[]
                {
                    new[] { "Alice", "alice@mail.com" },
                    new[] { "Bob", "bob@mail.com" },
                },
                new[]
                {
                    new[] { "Alice", "alice@mail.com" },
                    new[] { "Bob", "bob@mail.com" },
                }
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByPairwiseEmailScan_LeetCodeExamples_MergesAccountsSharingAnEmail(
        string[][] accounts, string[][] expected) =>
        AssertSameAccounts(expected, AccountsMergeSolution.MergeByPairwiseEmailScan(accounts));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByUnionFindByEmail_LeetCodeExamples_MergesAccountsSharingAnEmail(
        string[][] accounts, string[][] expected) =>
        AssertSameAccounts(expected, AccountsMergeSolution.MergeByUnionFindByEmail(accounts));

    private static void AssertSameAccounts(string[][] expected, List<string[]> actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        foreach (var account in expected)
        {
            Assert.Contains(actual, a => a.SequenceEqual(account));
        }
    }
}
