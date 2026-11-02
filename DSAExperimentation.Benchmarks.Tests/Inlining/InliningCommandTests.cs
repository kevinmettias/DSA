using DSAExperimentation.Benchmarks.Inlining;

namespace DSAExperimentation.Benchmarks.Tests.Inlining;

// The verb's arguments and refusals. Driving a real arm needs a Release build to show any inlines,
// and a test run is Debug, so the decisions themselves are InliningListenerTests' to state.
public sealed partial class InliningCommandTests
{
    private const int Usage = 2;
    private const string UsageLine = "usage: inlining";
    private const string RealClass = "WalkingRobotSimulation";
    private const string NoSuchName = "NoSuch";

    public static TheoryData<string[]> ArgumentsNamingNoArm =>
        new()
        {
            new[] { InliningCommand.Name },
            new[] { InliningCommand.Name, NoSuchName, NoSuchName },
            new[] { InliningCommand.Name, RealClass, NoSuchName },
        };

    [Fact]
    public void IsAskedFor_TheInliningVerb() => Assert.True(InliningCommand.IsAskedFor([InliningCommand.Name, RealClass, NoSuchName]));

    [Fact]
    public void IsAskedFor_NotAnotherVerb() => Assert.False(InliningCommand.IsAskedFor([NoSuchName, RealClass]));

    [Theory]
    [MemberData(nameof(ArgumentsNamingNoArm))]
    public async Task RunAsync_ArgumentsNamingNoArm_IsAUsageError(string[] args)
    {
        var error = new StringWriter();

        Assert.Equal(Usage, await InliningCommand.RunAsync(args, TextWriter.Null, error));
        Assert.Contains(UsageLine, error.ToString(), StringComparison.Ordinal);
    }
}
