using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AccountsMerge;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AccountsMergeSolution's, the same methods
// AccountsMergeTests proves correct - each now builds the actual merged accounts
// LeetCode asks for, rather than only counting the merged groups as the original
// pair of arms did. Emails are drawn from a shared pool sized AccountCount/5 so
// real overlaps - and therefore real merge work - actually occur, the same "force
// genuine matches, not coincidental ones" intent ReplaceWordsBenchmarks' generator
// already uses.
[MemoryDiagnoser]
public class AccountsMergeBenchmarks
{
    private const int RandomSeed = 721; // LC problem number
    private const int EmailPoolDivisor = 5;
    private const int MinEmailsPerAccount = 2;
    private const int EmailCountRange = 3;

    private string[][] _accounts = [];

    [Params(50, 400)]
    public int AccountCount { get; set; }

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
    public List<string[]> PairwiseEmailOverlapScan() =>
        AccountsMergeSolution.MergeByPairwiseEmailScan(_accounts);

    [Benchmark]
    public List<string[]> UnionFindByEmail() =>
        AccountsMergeSolution.MergeByUnionFindByEmail(_accounts);
}
