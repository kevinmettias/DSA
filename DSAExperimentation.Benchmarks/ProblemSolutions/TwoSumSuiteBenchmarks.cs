using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.TwoSum;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TwoSumSolution's, timed the way LeetCode times a submission - once
// across every case it holds rather than at one size. The cases are the LeetCode catalog's for LC 1:
// its published examples and LeetCodeDataset's, which are mostly small, so this shows what the arms
// cost on the inputs LeetCode poses, while TwoSumBenchmarks' sized workloads show how they grow.
// A generated case with several pairs breaks LC 1's promise and has several right answers, so only
// the cases TwoSumPromise holds for are timed - the inputs LeetCode itself could pose. Parsing the
// catalog's notation happens in [GlobalSetup], outside the timed region, and each arm writes its
// answers into a buffer sized there.
public class TwoSumSuiteBenchmarks
{
    private const string TitleSlug = "two-sum";

    private (int[] Nums, int Target)[] _cases = [];
    private (bool Found, int First, int Second)[] _answers = [];

    [GlobalSetup]
    public void Setup()
    {
        _cases =
        [
            .. LeetCodeCatalog.Cases(TitleSlug)
                .Select(testCase => (Nums: testCase.Input<int[]>(0), Target: testCase.Input<int>(1)))
                .Where(input => TwoSumPromise.IsKeptBy(input.Nums, input.Target)),
        ];
        _answers = new (bool Found, int First, int Second)[_cases.Length];
    }

    [Benchmark(Baseline = true)]
    public (bool Found, int First, int Second)[] TryFindIndicesByBruteForce()
    {
        for (var index = 0; index < _cases.Length; index++)
        {
            var (nums, target) = _cases[index];
            var found = TwoSumSolution.TryFindIndicesByBruteForce(nums, target, out var first, out var second);
            _answers[index] = (found, first, second);
        }

        return _answers;
    }

    [Benchmark]
    public (bool Found, int First, int Second)[] TryFindIndicesByHashMap()
    {
        for (var index = 0; index < _cases.Length; index++)
        {
            var (nums, target) = _cases[index];
            var found = TwoSumSolution.TryFindIndicesByHashMap(nums, target, out var first, out var second);
            _answers[index] = (found, first, second);
        }

        return _answers;
    }
}
