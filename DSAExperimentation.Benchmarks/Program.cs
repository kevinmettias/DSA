using System.Reflection;

using BenchmarkDotNet.Running;

using DSAExperimentation.Benchmarks;

// The config is built from the arguments rather than attached as an assembly attribute, because
// whether the pinned recording job applies depends on what the caller asked for - see
// BenchmarkConfig.For.
BenchmarkSwitcher
    .FromAssembly(Assembly.GetExecutingAssembly())
    .Run(args, BenchmarkConfig.For(args));
