using System.Reflection;

using BenchmarkDotNet.Running;

using DSAExperimentation.Benchmarks;
using DSAExperimentation.Benchmarks.Baseline;

// The config is built from the arguments rather than attached as an assembly attribute, because
// whether the pinned recording job applies depends on what the caller asked for - see
// BenchmarkConfig.For.
//
// The `baseline` verb is claimed before BenchmarkDotNet sees anything, because it is the one
// thing this executable does that is not "run the benchmarks": it is what puts a run in the
// repository and what checks a later one against it, and both of those set an exit code a
// caller has to be able to branch on.
if (BaselineCommand.Handles(args))
{
    return BaselineCommand.Run(args, Console.Out, Console.Error);
}

BenchmarkSwitcher
    .FromAssembly(Assembly.GetExecutingAssembly())
    .Run(args, BenchmarkConfig.For(args));

return 0;
