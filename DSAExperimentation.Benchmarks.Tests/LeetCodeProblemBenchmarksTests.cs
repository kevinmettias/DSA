using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests;

// Harness coverage for LeetCodeProblemBenchmarks (ARCHITECTURE 17.9): the one benchmark class that stands in for
// every registered problem. Its arm is a string, resolved once in [GlobalSetup] into an already-bound workload,
// so the properties worth asserting are the registry's own - the arm list is a non-empty set of well-formed
// three-part names that enumerates identically twice, Setup rejects anything that is not one of them, and Run
// answers only after an arm has actually been bound. Running one bound arm twice is the agreement family here:
// the same arm must hand back the same answer, which is what makes a published number a property of the arm
// rather than of when it ran.
public sealed partial class LeetCodeProblemBenchmarksTests
{
    private const char ArmSeparator = '/';
    private const int ArmPartCount = 3;
    private const int FewestRegisteredArms = 1;
    private const string GlobalSetupMarker = "[GlobalSetup]";

    // A known arm whose answer is decisive: LC 1's unreachable-target workload, whose values are all positive
    // and whose target is negative, so neither strategy can find a pair and both answer an empty index array.
    private const string UnreachableTargetArm = "two-sum/BruteForce/unreachable-target-200";
    private const string MalformedArm = "two-sum/BruteForce";

    [Fact]
    public void Arms_RegisteredWorkloads_ReturnsWellFormedThreePartArmNames()
    {
        var arms = LeetCodeProblemBenchmarks.Arms.ToList();

        Assert.True(arms.Count >= FewestRegisteredArms);
        Assert.All(arms, arm => Assert.Equal(ArmPartCount, arm.Split(ArmSeparator).Length));
    }

    [Fact]
    public void Arms_SameRegistry_EnumeratesTheSameArmsTwice() =>
        Assert.Equal(LeetCodeProblemBenchmarks.Arms, LeetCodeProblemBenchmarks.Arms);

    [Fact]
    public void Setup_AnArmThatIsNotAProblemStrategyWorkloadTriple_NamesWhatItIsNot()
    {
        var harness = new LeetCodeProblemBenchmarks { Arm = MalformedArm };

        var exception = Record.Exception(harness.Setup);

        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains(MalformedArm, exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Setup_RegisteredArm_BindsItSoRunAnswersTheWorkloadsOwnResult()
    {
        var harness = new LeetCodeProblemBenchmarks { Arm = UnreachableTargetArm };
        harness.Setup();

        Assert.Empty(Assert.IsType<int[]>(harness.Run()));
    }

    [Fact]
    public void Run_BoundArm_AnswersTheSameBothTimes()
    {
        var harness = new LeetCodeProblemBenchmarks { Arm = UnreachableTargetArm };
        harness.Setup();

        Assert.Equal(AnswerText.Of(harness.Run()), AnswerText.Of(harness.Run()));
    }

    [Fact]
    public void Run_WithoutSetup_NamesTheMissingSetupStep()
    {
        var harness = new LeetCodeProblemBenchmarks { Arm = UnreachableTargetArm };

        var exception = Record.Exception(harness.Run);

        Assert.IsType<InvalidOperationException>(exception);
        Assert.Contains(GlobalSetupMarker, exception.Message, StringComparison.Ordinal);
    }
}
