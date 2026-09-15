using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.AccountsMerge;

// LeetCode 721. Accounts Merge: two accounts belong together when they share an
// email; each merged group reports the owner's name once and every distinct email
// it collectively owns, sorted (ordinal, not the culture-aware default comparer,
// which - verified in this repo's own en-US test environment - does not agree with
// '_' < '0' < 's' character-code order).
//
// The two strategies differ in how "do these accounts belong together" is decided -
// an O(accounts^2 * emails^2) pairwise scan comparing every account against every
// other, or first-seen-owner union-find (a HashMap<email,accountIndex> unions the
// moment a second account reaches an already-seen email) in O(totalEmails) - the
// same RedundantConnectionTests union-on-shared-element shape applied to emails
// instead of graph edges.
internal static class AccountsMergeSolution
{
    // The textbook answer: decide "same person" by scanning every email of account i
    // against every email of account j for every account pair, entirely with BCL
    // collections - the arm the union-find strategy below has to justify itself
    // against.
    public static List<string[]> MergeByPairwiseEmailScan(string[][] accounts)
    {
        var parent = BuildIdentityParent(accounts.Length);
        UnionSharedEmailPairs(parent, accounts);

        return BuildMergedAccounts(accounts, new RootByParentWalk(parent));
    }

    private static int[] BuildIdentityParent(int size)
    {
        var parent = new int[size];

        for (var i = 0; i < parent.Length; i++)
        {
            parent[i] = i;
        }

        return parent;
    }

    private static void UnionSharedEmailPairs(int[] parent, string[][] accounts)
    {
        for (var i = 0; i < accounts.Length; i++)
        {
            for (var j = i + 1; j < accounts.Length; j++)
            {
                if (SharesEmail(accounts[i], accounts[j]))
                {
                    Union(parent, i, j);
                }
            }
        }
    }

    private static bool SharesEmail(string[] first, string[] second)
    {
        for (var i = 1; i < first.Length; i++)
        {
            for (var j = 1; j < second.Length; j++)
            {
                if (first[i] == second[j])
                {
                    return true;
                }
            }
        }

        return false;
    }

    // The question the pairwise arm's merge phase asks about the parent array
    // BuildIdentityParent wrote: which account index is the canonical root of the
    // group this account merged into. The answer is pure and idempotent - the root of
    // a root is itself - and is defined for every index in 0..accounts.Length-1,
    // including one that merged with nothing, whose root is its own index.
    private interface IAccountRootLookup
    {
        int FindRoot(int accountIndex);
    }

    // The pairwise arm's answer: walk the parent array to its root, one hop at a
    // time, over the identity array BuildIdentityParent wrote.
    private sealed class RootByParentWalk : IAccountRootLookup
    {
        private readonly int[] parent;

        public RootByParentWalk(int[] parent) => this.parent = parent;

        public int FindRoot(int accountIndex) => Find(parent, accountIndex);
    }

    private static List<string[]> BuildMergedAccounts(string[][] accounts, IAccountRootLookup roots)
    {
        var emailsByRoot = new Dictionary<int, HashSet<string>>();
        var nameByRoot = new Dictionary<int, string>();

        for (var i = 0; i < accounts.Length; i++)
        {
            var root = roots.FindRoot(i);

            if (!emailsByRoot.TryGetValue(root, out var emails))
            {
                emails = new HashSet<string>();
                emailsByRoot[root] = emails;
                nameByRoot[root] = accounts[i][0];
            }

            for (var j = 1; j < accounts[i].Length; j++)
            {
                emails.Add(accounts[i][j]);
            }
        }

        return MaterializeMergedAccounts(emailsByRoot, nameByRoot);
    }

    // One merged row per root, in dictionary order: its owner's name followed by
    // every distinct email that root collected.
    private static List<string[]> MaterializeMergedAccounts(
        Dictionary<int, HashSet<string>> emailsByRoot, Dictionary<int, string> nameByRoot)
    {
        var merged = new List<string[]>();

        foreach (var (root, emails) in emailsByRoot)
        {
            var account = BuildAccountRow(root, nameByRoot[root], emails);
            merged.Add(account);
        }

        return merged;
    }

    // One merged row: the owner's name, then every distinct email its root
    // collected, sorted ordinal.
    private static string[] BuildAccountRow(int root, string name, HashSet<string> emails)
    {
        var sortedEmails = emails.ToArray();
        Array.Sort(sortedEmails, StringComparer.Ordinal);

        var account = new string[sortedEmails.Length + 1];
        account[0] = name;
        Array.Copy(sortedEmails, 0, account, 1, sortedEmails.Length);

        return account;
    }

    // This repo's own DisjointSet, unioned by first-seen email owner (tracked via
    // HashMap<email,accountIndex>): sharing an email is discovered in O(1) the
    // moment a second account reaches that same email, never comparing two
    // accounts directly. Emails per merged root are deduped via
    // HashMap<email,bool> and sorted with this repo's MergeSort over
    // ArrayIndexedSequence.
    public static List<string[]> MergeByUnionFindByEmail(string[][] accounts)
    {
        var components = BuildDisjointSetFromSharedEmails(accounts);
        var emailsByRoot = GroupEmailsByRoot(accounts, components);

        var merged = new List<string[]>();

        foreach (var root in emailsByRoot.Keys)
        {
            var account = BuildMergedAccount(root, emailsByRoot, accounts);
            merged.Add(account);
        }

        return merged;
    }

    private static DisjointSet BuildDisjointSetFromSharedEmails(string[][] accounts)
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

        return components;
    }

    private static HashMap<int, HashMap<string, bool>> GroupEmailsByRoot(string[][] accounts, DisjointSet components)
    {
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

        return emailsByRoot;
    }

    private static string[] BuildMergedAccount(int root, HashMap<int, HashMap<string, bool>> emailsByRoot, string[][] accounts)
    {
        emailsByRoot.TryGetValue(root, out var emails);
        var sortedEmails = emails.Keys.ToArray();
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(sortedEmails), StringComparer.Ordinal);

        var account = new string[sortedEmails.Length + 1];
        account[0] = accounts[root][0];
        Array.Copy(sortedEmails, 0, account, 1, sortedEmails.Length);
        return account;
    }

    private static int Find(int[] parent, int id)
    {
        while (parent[id] != id)
        {
            id = parent[id];
        }

        return id;
    }

    private static void Union(int[] parent, int first, int second)
    {
        var firstRoot = Find(parent, first);
        var secondRoot = Find(parent, second);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }
}
