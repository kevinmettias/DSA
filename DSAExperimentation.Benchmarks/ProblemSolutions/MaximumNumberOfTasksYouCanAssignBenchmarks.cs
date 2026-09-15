using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumNumberOfTasksYouCanAssign;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumNumberOfTasksYouCanAssignSolution's, the same
// methods MaximumNumberOfTasksYouCanAssignTests proves correct - a linear walk down
// "try k = maxK, maxK - 1, ..." against this repo's own BinarySearch.LowerBound over
// an on-demand IRandomAccessSequence<bool> feasibility sequence, the same shape
// KokoEatingBananasBenchmarks measures, so the comparison is O(maxK) feasibility
// checks against O(log maxK) of them. Tasks are drawn from a strength range far
// above what any worker can reach even with every pill, so the true answer is always
// 0 - the worst case for the linear scan, which must walk all the way down from maxK
// before stopping. [GlobalSetup] sorts both arrays into the prepared
// SortedTaskAssignment its hoisted overload takes, so sorting is not charged to
// either measured arm.
[MemoryDiagnoser]
public class MaximumNumberOfTasksYouCanAssignBenchmarks
{
    private const int RandomSeed = 2071; // LC problem number
    private const int MinWorkerStrength = 1;
    private const int MaxWorkerStrengthExclusive = 100;
    private const int MinTaskRequirement = 500_000;
    private const int MaxTaskRequirementExclusive = 1_000_000;
    private const int Pills = 5;
    private const int Strength = 100;

    private MaximumNumberOfTasksYouCanAssignSolution.SortedTaskAssignment _assignment;

    [Params(2_000, 50_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var tasks = Enumerable.Range(0, Length)
            .Select(_ => random.Next(MinTaskRequirement, MaxTaskRequirementExclusive))
            .Order()
            .ToArray();
        var workers = Enumerable.Range(0, Length)
            .Select(_ => random.Next(MinWorkerStrength, MaxWorkerStrengthExclusive))
            .Order()
            .ToArray();

        _assignment = new MaximumNumberOfTasksYouCanAssignSolution.SortedTaskAssignment(tasks, workers, Pills, Strength);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() => MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentByLinearScan(_assignment);

    [Benchmark]
    public int SequenceLowerBound() => MaximumNumberOfTasksYouCanAssignSolution.MaxTaskAssignmentBySequenceLowerBound(_assignment);
}
