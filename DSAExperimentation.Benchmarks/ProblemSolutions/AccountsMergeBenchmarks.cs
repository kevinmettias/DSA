using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AccountsMerge;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AccountsMergeSolution's, the same methods
// AccountsMergeTests proves correct - each now builds the actual merged accounts
// LeetCode asks for, rather than only counting the merged groups as the original
// pair of arms did. Emails are drawn from a shared per-person pool so real overlaps -
// and therefore real merge work - actually occur, the same "force genuine matches,
// not coincidental ones" intent ReplaceWordsBenchmarks' generator already uses.
//
// Each account draws both its name and its emails from one person's slice of the pool.
// Two accounts that share an email therefore share an owner and report the same name,
// which is what LeetCode 721 assumes when it says a merged group reports the owner's
// name once. An earlier version drew every account's name independently (Person0,
// Person1, ...) and only its emails from the shared pool, so two accounts with the
// same email could carry different names - input outside the problem's contract, on
// which the two arms legitimately disagree about whose name survives, which made the
// benchmark time a question the tests never settled.
[MemoryDiagnoser]
public class AccountsMergeBenchmarks
{
    private const int RandomSeed = 721; // LC problem number
    private const int PersonCountDivisor = 5;
    private const int EmailsPerPerson = 4;
    private const int MinEmailsPerAccount = 2;
    private const int EmailCountRange = 3;

    private string[][] _accounts = [];

    [Params(50, 400)]
    public int AccountCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var personCount = Math.Max(1, AccountCount / PersonCountDivisor);

        _accounts = Enumerable.Range(0, AccountCount)
            .Select(i => BuildAccount(random, personCount))
            .ToArray();
    }

    private static string[] BuildAccount(Random random, int personCount)
    {
        var owner = random.Next(personCount);
        var emailCount = MinEmailsPerAccount + random.Next(EmailCountRange);
        var emails = Enumerable.Range(0, emailCount)
            .Select(_ => $"user{owner}_{random.Next(EmailsPerPerson)}@mail.com")
            .Distinct();

        return [$"Person{owner}", .. emails];
    }

    [Benchmark(Baseline = true)]
    public List<string[]> PairwiseEmailOverlapScan() =>
        AccountsMergeSolution.MergeByPairwiseEmailScan(_accounts);

    [Benchmark]
    public List<string[]> UnionFindByEmail() =>
        AccountsMergeSolution.MergeByUnionFindByEmail(_accounts);
}
