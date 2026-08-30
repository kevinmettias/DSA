using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AccountsMerge;

// LeetCode 721. Accounts Merge: DisjointSet over account indices, unioned whenever two
// accounts share an email (first-seen owner tracked via HashMap<email,accountIndex>,
// the same RedundantConnectionTests union-on-shared-element shape applied to emails
// instead of graph edges). Each merged root's emails are deduped via a
// HashMap<email,bool> and sorted with this repo's MergeSort over ArrayIndexedSequence
// - the same "sort with this repo's MergeSort" convention ThreeSumTests already uses.
// Ordinal, not the default comparer: Comparer<string>.Default is culture-aware and
// (verified in this repo's own en-US test environment) does not agree with '_' < '0'
// < 's' character-code order, so LeetCode's own "sorted" output would silently mismatch
// a culture-collated one on some machines/CI locales.
public sealed partial class AccountsMergeTests
{
    [Fact]
    public void Merge_ClassicExample_MergesAccountsSharingAnEmail()
    {
        string[][] accounts =
        [
            ["John", "johnsmith@mail.com", "john_newyork@mail.com"],
            ["John", "johnsmith@mail.com", "john00@mail.com"],
            ["Mary", "mary@mail.com"],
            ["John", "johnnybravo@mail.com"],
        ];

        var merged = Merge(accounts);

        Assert.Equal(3, merged.Count);
        Assert.Contains(merged, a => a.SequenceEqual(new[] { "John", "john00@mail.com", "john_newyork@mail.com", "johnsmith@mail.com" }));
        Assert.Contains(merged, a => a.SequenceEqual(new[] { "Mary", "mary@mail.com" }));
        Assert.Contains(merged, a => a.SequenceEqual(new[] { "John", "johnnybravo@mail.com" }));
    }

    [Fact]
    public void Merge_NoSharedEmails_KeepsEachAccountSeparate()
    {
        string[][] accounts =
        [
            ["Alice", "alice@mail.com"],
            ["Bob", "bob@mail.com"],
        ];

        var merged = Merge(accounts);

        Assert.Equal(2, merged.Count);
    }

    private static List<string[]> Merge(string[][] accounts)
    {
        var components = new DisjointSet(accounts.Length);
        var firstOwner = new HashMap<string, int>();

        for (var i = 0; i < accounts.Length; i++)
        {
            for (var j = 1; j < accounts[i].Length; j++)
            {
                var email = accounts[i][j];

                if (firstOwner.TryGetValue(email, out var owner))
                {
                    components.Union(i, owner);
                }
                else
                {
                    firstOwner.Set(email, i);
                }
            }
        }

        var emailsByRoot = new HashMap<int, HashMap<string, bool>>();

        for (var i = 0; i < accounts.Length; i++)
        {
            var root = components.Find(i);

            if (!emailsByRoot.TryGetValue(root, out var emails))
            {
                emails = new HashMap<string, bool>();
                emailsByRoot.Set(root, emails);
            }

            for (var j = 1; j < accounts[i].Length; j++)
            {
                emails.Set(accounts[i][j], true);
            }
        }

        var merged = new List<string[]>();

        foreach (var root in emailsByRoot.Keys)
        {
            emailsByRoot.TryGetValue(root, out var emails);
            var sortedEmails = emails.Keys.ToArray();
            MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(sortedEmails), StringComparer.Ordinal);

            var account = new string[sortedEmails.Length + 1];
            account[0] = accounts[root][0];
            Array.Copy(sortedEmails, 0, account, 1, sortedEmails.Length);
            merged.Add(account);
        }

        return merged;
    }
}
