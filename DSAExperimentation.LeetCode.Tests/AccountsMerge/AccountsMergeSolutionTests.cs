using DSAExperimentation.LeetCode.AccountsMerge;

namespace DSAExperimentation.LeetCode.Tests.AccountsMerge;

// Harness only. Both strategies are AccountsMergeSolution's - this file just pins
// them to LeetCode's published examples. Merged-account order is unspecified by
// LeetCode, so examples are checked as a set of expected accounts rather than by
// position.
public sealed partial class AccountsMergeSolutionTests
{
    public static TheoryData<string[][], string[][]> Examples =>
        new()
        {
            // LeetCode examples 1 and 2.
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
                    new[] { "Gabe", "Gabe0@m.co", "Gabe3@m.co", "Gabe1@m.co" },
                    new[] { "Kevin", "Kevin3@m.co", "Kevin5@m.co", "Kevin0@m.co" },
                    new[] { "Ethan", "Ethan5@m.co", "Ethan4@m.co", "Ethan0@m.co" },
                    new[] { "Hanzo", "Hanzo3@m.co", "Hanzo1@m.co", "Hanzo0@m.co" },
                    new[] { "Fern", "Fern5@m.co", "Fern1@m.co", "Fern0@m.co" },
                },
                new[]
                {
                    new[] { "Ethan", "Ethan0@m.co", "Ethan4@m.co", "Ethan5@m.co" },
                    new[] { "Gabe", "Gabe0@m.co", "Gabe1@m.co", "Gabe3@m.co" },
                    new[] { "Hanzo", "Hanzo0@m.co", "Hanzo1@m.co", "Hanzo3@m.co" },
                    new[] { "Kevin", "Kevin0@m.co", "Kevin3@m.co", "Kevin5@m.co" },
                    new[] { "Fern", "Fern0@m.co", "Fern1@m.co", "Fern5@m.co" },
                }
            },

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
