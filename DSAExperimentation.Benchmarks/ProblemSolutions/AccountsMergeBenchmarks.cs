using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Accounts Merge (LC 721): the naive pairwise approach that decides "do these two
// accounts belong together" by scanning every email of account i against every email
// of account j for every account pair - O(accounts^2 * emailsPerAccount^2) - vs. this
// repo's DisjointSet unioned by first-seen email owner (tracked in a HashMap<email,
// accountIndex>), which never compares two accounts directly at all: sharing an email
// is discovered in O(1) via the map the moment the second account reaches that same
// email. O(totalEmails) total. Emails are drawn from a shared pool sized
// AccountCount/5 so real overlaps - and therefore real merge work - actually occur,
// the same "force genuine matches, not coincidental ones" intent
// ReplaceWordsBenchmarks' generator already uses.
[MemoryDiagnoser]
public class AccountsMergeBenchmarks
{
    private const int RandomSeed = 721; // LC problem number
    private const int EmailPoolDivisor = 5;
    private const int MinEmailsPerAccount = 2;
    private const int EmailCountRange = 3;

    [Params(50, 400)]
    public int AccountCount;

    private string[][] _accounts = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var emailPoolSize = Math.Max(1, AccountCount / EmailPoolDivisor);
        var emailPool = Enumerable.Range(0, emailPoolSize)
            .Select(i => $"user{i}@mail.com")
            .ToArray();

        _accounts = Enumerable.Range(0, AccountCount)
            .Select(i =>
            {
                var emailCount = MinEmailsPerAccount + random.Next(EmailCountRange);
                var emails = Enumerable.Range(0, emailCount).Select(_ => emailPool[random.Next(emailPool.Length)]).Distinct();
                return new[] { $"Person{i}" }.Concat(emails).ToArray();
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseEmailOverlapScan()
    {
        var parent = BuildIdentityParent(_accounts.Length);
        UnionSharedEmailPairs(parent, _accounts);
        return CountDistinctRoots(parent);
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

    private static int CountDistinctRoots(int[] parent)
    {
        var roots = new HashSet<int>();

        for (var i = 0; i < parent.Length; i++)
        {
            var root = Find(parent, i);
            roots.Add(root);
        }

        return roots.Count;
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

    [Benchmark]
    public int UnionFindByEmail()
    {
        var components = new DisjointSet(_accounts.Length);
        var firstOwner = new HashMap<string, int>();
        UnionAccountsBySharedEmail(_accounts, components, firstOwner);
        return CountDisjointSetRoots(components, _accounts.Length);
    }

    private static void UnionAccountsBySharedEmail(string[][] accounts, DisjointSet components, HashMap<string, int> firstOwner)
    {
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
    }

    private static int CountDisjointSetRoots(DisjointSet components, int accountCount)
    {
        var roots = new HashMap<int, bool>();

        for (var i = 0; i < accountCount; i++)
        {
            roots.Set(components.Find(i), true);
        }

        return roots.Count;
    }
}
