using DSAExperimentation.LeetCode.AccountsMerge;

namespace DSAExperimentation.LeetCode.Tests.AccountsMerge;

// The seam between AccountsMergeSolution's two mergers.
// MergeByPairwiseEmailScan compares every pair of accounts to decide whether they
// share an email, then walks a hand-rolled parent array to find each group's root.
// MergeByUnionFindByEmail composes three of this repo's own structures instead:
// DataStructures.DisjointSet collapses groups by first-seen email owner tracked in a
// DataStructures.HashMap<string,int>, emails are deduped into a
// HashMap<int,HashMap<string,bool>>, and each merged row's emails reach their sorted
// order through Algorithms.Sorting.MergeSort over an
// DataStructures.Sequence.ArrayIndexedSequence<string>.
//
// Two parts of that composition are observable from outside. The union-find decides
// which accounts land in the same row, and the answer must be identical no matter
// which account happened to own a shared email first - that is the transitivity of
// the equivalence relation, and a union that missed a link leaves two rows where
// there should be one. The merge sort decides the order inside every row, and it is
// passed StringComparer.Ordinal explicitly, so a row's emails must come back in
// ordinal order rather than in whatever order a culture-aware comparison would give.
public sealed partial class UnionFindEmailMergeSeamTests
{
    [Fact]
    public void Merge_LeetCodeExample_MatchesPairwiseScan()
    {
        string[][] accounts =
        [
            ["John", "johnsmith@mail.com", "john_newyork@mail.com"],
            ["John", "johnsmith@mail.com", "john00@mail.com"],
            ["Mary", "mary@mail.com"],
            ["John", "johnnybravo@mail.com"],
        ];

        Assert.Equal(
            [
                "John|john00@mail.com,john_newyork@mail.com,johnsmith@mail.com",
                "John|johnnybravo@mail.com",
                "Mary|mary@mail.com",
            ],
            Normalize(MergeByUnionFindArm(accounts)));
        AssertSameRows(accounts);
    }

    // Ordinal order is the sort contract the composed arm's MergeSort is handed, and
    // it is not what a culture-aware comparison would produce: ordinal compares code
    // points, so digits sort before letters, every uppercase letter sorts before
    // every lowercase one, and "@" is not a separator the comparison knows about.
    [Fact]
    public void Merge_MixedCaseAndDigitEmails_SortsOrdinalWithinEveryRow()
    {
        string[][] accounts =
        [
            ["Casey", "zulu@m.co", "Alpha@m.co", "9lives@m.co", "alpha@m.co", "Zebra@m.co"],
            ["Casey", "alpha@m.co", "banana@m.co"],
        ];

        Assert.Equal(
            [
                "Casey|9lives@m.co,Alpha@m.co,Zebra@m.co,alpha@m.co,banana@m.co,zulu@m.co",
            ],
            Normalize(MergeByUnionFindArm(accounts)));
        AssertSameRows(accounts);
    }

    // A merge that is only discoverable by chaining: no two accounts beyond adjacent
    // ones share an email, so each Union links a pair whose roots were already joined
    // by the previous Union. Every account has to reach the same root, which is the
    // property a union-find that stopped one hop short of the root would lose.
    [Fact]
    public void Merge_LongChainOfSingleSharedEmails_CollapsesToOneRow()
    {
        const int AccountCount = 40;
        var accounts = new string[AccountCount + 1][];
        accounts[0] = ["P", "e0@m.co"];

        for (var i = 1; i <= AccountCount; i++)
        {
            accounts[i] = ["P", $"e{i - 1}@m.co", $"e{i}@m.co"];
        }

        var merged = MergeByUnionFindArm(accounts);

        Assert.Single(merged);
        Assert.Equal(
            OrdinalEmails([.. Enumerable.Range(0, AccountCount + 1).Select(i => $"e{i}@m.co")]),
            merged[0][1..]);
        AssertSameRows(accounts);
    }

    // Two accounts with no email in common must stay apart even when they carry the
    // same name: the problem states two accounts only belong to the same person when
    // they share an email, so a merger keyed on the name would merge these.
    [Fact]
    public void Merge_TwoAccountsWithTheSameNameAndNoSharedEmail_StaySeparate()
    {
        string[][] accounts =
        [
            ["Sam", "sam.work@m.co"],
            ["Sam", "sam.home@m.co"],
        ];

        Assert.Equal(["Sam|sam.home@m.co", "Sam|sam.work@m.co"], Normalize(MergeByUnionFindArm(accounts)));
        AssertSameRows(accounts);
    }

    // The same email listed twice inside one account, and a third account reaching it
    // too: the composed arm's HashMap<string,bool> dedupe has to leave one copy of the
    // repeated email while still merging all three accounts into a single row.
    [Fact]
    public void Merge_EmailRepeatedWithinAndAcrossAccounts_DedupesAndMerges()
    {
        string[][] accounts =
        [
            ["Dup", "shared@m.co", "shared@m.co", "first@m.co"],
            ["Dup", "shared@m.co"],
            ["Dup", "shared@m.co", "second@m.co"],
        ];

        Assert.Equal(
            ["Dup|first@m.co,second@m.co,shared@m.co"],
            Normalize(MergeByUnionFindArm(accounts)));
        AssertSameRows(accounts);
    }

    // A group whose merge is only visible through a link in the middle of the list:
    // the accounts before and after the link are in different groups until the middle
    // one is read, so neither arm may decide a row the moment it sees the first email.
    [Fact]
    public void Merge_GroupLinkedOnlyByAMiddleAccount_MatchesPairwiseScan()
    {
        string[][] accounts =
        [
            ["Link", "left@m.co"],
            ["Link", "middle@m.co"],
            ["Link", "right@m.co"],
            ["Other", "alone@m.co"],
            ["Link", "left@m.co", "middle@m.co"],
            ["Link", "middle@m.co", "right@m.co"],
        ];

        Assert.Equal(
            ["Link|left@m.co,middle@m.co,right@m.co", "Other|alone@m.co"],
            Normalize(MergeByUnionFindArm(accounts)));
        AssertSameRows(accounts);
    }

    [Fact]
    public void Merge_NoAccounts_ReturnsNothingFromEitherArm()
    {
        string[][] accounts = [];

        Assert.Empty(MergeByUnionFindArm(accounts));
        Assert.Empty(AccountsMergeSolution.MergeByPairwiseEmailScan(accounts));
    }

    private static void AssertSameRows(string[][] accounts)
        => Assert.Equal(Normalize(AccountsMergeSolution.MergeByPairwiseEmailScan(accounts)), Normalize(MergeByUnionFindArm(accounts)));

    private static List<string[]> MergeByUnionFindArm(string[][] accounts)
        => AccountsMergeSolution.MergeByUnionFindByEmail(accounts);

    // The two arms group by different roots and visit groups in different orders, so
    // their rows are compared as an unordered set of rows with each row's own email
    // order preserved - an ordering difference inside a row is a real difference.
    private static List<string> Normalize(List<string[]> merged)
        => [.. merged.Select(row => $"{row[0]}|{string.Join(',', row[1..])}").OrderBy(row => row, StringComparer.Ordinal)];

    private static string[] OrdinalEmails(string[] emails)
        => [.. emails.OrderBy(email => email, StringComparer.Ordinal)];
}
