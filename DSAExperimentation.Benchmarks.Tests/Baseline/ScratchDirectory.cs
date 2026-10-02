namespace DSAExperimentation.Benchmarks.Tests.Baseline;

// A directory of its own, under the system temporary folder, removed when the test that made
// it ends.
//
// The baseline command takes every path it touches from its command line, so a test can drive
// the whole record-then-compare loop with two files in here, and never near the repository's
// own baseline or the artifacts directory its benchmarks write to. That is only true because
// of the `--report` option: without it, both modes would reach for BenchmarkDotNet and the
// test would be timing the machine rather than checking the arithmetic.
internal sealed class ScratchDirectory : IDisposable
{
    public ScratchDirectory()
    {
        Root = Path.Combine(Path.GetTempPath(), $"dsa-baseline-{Guid.NewGuid():N}");
        Directory.CreateDirectory(Root);
    }

    public string Root { get; }

    public string At(string name) => Path.Combine(Root, name);

    public string Write(string name, string contents)
    {
        var path = At(name);
        File.WriteAllText(path, contents);

        return path;
    }

    public void Dispose()
    {
        if (Directory.Exists(Root))
        {
            Directory.Delete(Root, recursive: true);
        }
    }
}
