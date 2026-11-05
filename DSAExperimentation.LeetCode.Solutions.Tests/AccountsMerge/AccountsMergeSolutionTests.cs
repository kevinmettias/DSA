using DSAExperimentation.LeetCode.Solutions.AccountsMerge;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AccountsMerge;

// Harness only. Both strategies are AccountsMergeSolution's - this file pins them to
// a case the catalog lacks: two people with no shared email, so nothing merges.
// Merged-account order is unspecified by LeetCode, so cases are checked as a set of
// expected accounts rather than by position. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class AccountsMergeSolutionTests
{
    public static TheoryData<string[][], string[][]> Cases =>
        new()
        {
            // Two people, no shared email: nothing merges.
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
    [MemberData(nameof(Cases))]
    public void MergeByPairwiseEmailScan_Cases_MergesAccountsSharingAnEmail(
        string[][] accounts, string[][] expected) =>
        AssertSameAccounts(expected, AccountsMergeSolution.MergeByPairwiseEmailScan(accounts));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MergeByUnionFindByEmail_Cases_MergesAccountsSharingAnEmail(
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
