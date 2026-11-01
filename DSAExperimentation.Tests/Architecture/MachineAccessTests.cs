using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.6c: nothing below the harness touches the machine. The
// library's tiers and the solutions hold only state their caller owns - no file, socket, clock read,
// thread, console, ambient randomness or process-wide setting - so every answer is a function of its
// inputs and every test of one repeats. Randomness enters as a seeded Random the caller passes in.
// The harnesses are where a run meets the machine - the benchmark baseline tool's files and clock,
// the LeetCode catalog's network and disk - and are not scanned. Comments and strings are blanked
// first, so prose that names an API is not mistaken for code that calls it.
public sealed partial class MachineAccessTests
{
    private static readonly string[] ScannedProjects = ["DSAExperimentation", "DSAExperimentation.LeetCode"];

    // What each kind of machine access looks like in code, named for the finding it produces.
    private static readonly MachineAccess[] Accesses =
    [
        new("the file system", new Regex(
            @"\b(?:File|Directory)\s*\.\s*(?:Read|Write|Append|Open|Create|Delete|Copy|Move|Exists|Enumerate|Get|Set|Replace)\w*"
            + @"|\b(?:FileStream|StreamReader|StreamWriter|FileInfo|DirectoryInfo|DriveInfo|FileSystemWatcher)\b"
            + @"|\bPath\s*\.\s*(?:GetTempPath|GetTempFileName|GetFullPath)\b")),
        new("the network", new Regex(
            @"\b(?:HttpClient|HttpWebRequest|WebRequest|WebClient|Socket|TcpClient|TcpListener|UdpClient|Dns|NetworkStream)\b")),
        new("the clock", new Regex(
            @"\b(?:DateTime|DateTimeOffset)\s*\.\s*(?:Now|UtcNow|Today)\b|\bStopwatch\b"
            + @"|\bEnvironment\s*\.\s*TickCount(?:64)?\b|\bTimeProvider\s*\.\s*System\b")),
        new("a thread", new Regex(
            @"\b(?:Thread|ThreadPool|Parallel)\s*\.|\bnew\s+Thread\s*\(|\bTask\s*\.\s*(?:Run|Delay|Factory|Yield|WhenAll|WhenAny)\b")),
        new("the console", new Regex(@"\bConsole\s*\.")),
        new("ambient randomness", new Regex(
            @"\bnew\s+Random\s*\(\s*\)|\bRandom\s+\w+\s*=\s*new\s*\(\s*\)|\bRandom\s*\.\s*Shared\b"
            + @"|\bRandomNumberGenerator\b|\bGuid\s*\.\s*NewGuid\b")),
        new("the process", new Regex(
            @"\bEnvironment\s*\.\s*(?!TickCount)\w+|\bProcess\s*\.\s*(?:Start|GetCurrentProcess|GetProcesses)\b")),
    ];

    public static TheoryData<string> AccessesInCode =>
        new()
        {
            "var text = File.ReadAllText(path);",
            "using var reader = new StreamReader(stream);",
            "var client = new HttpClient();",
            "var now = DateTime.Now;",
            "var watch = Stopwatch.StartNew();",
            "Task.Run(() => Work());",
            "Console.WriteLine(answer);",
            "var random = new Random();",
            "private readonly Random _random = new();",
            "var pick = Random.Shared.Next(n);",
            "var home = Environment.GetEnvironmentVariable(name);",
        };

    // A seeded Random is the caller's randomness, not the machine's; the rest only look like access.
    public static TheoryData<string> LookalikesOfMachineAccess =>
        new()
        {
            "var random = new Random(seed);",
            "// a harness reads DateTime.Now, a solution never does",
            "var message = \"Console.WriteLine\";",
            "var count = folder.Files.Count;",
            "var day = schedule.Today;",
        };

    [Fact]
    public void LibraryAndSolutions_TouchNoMachine()
    {
        var root = RepositoryFiles.Root();
        var offences = ScannedProjects
            .SelectMany(project => RepositoryFiles.SourceFilesIn(Path.Combine(root, project)))
            .SelectMany(file => MachineAccessIn(File.ReadAllText(file))
                .Select(touched => $"{RepositoryFiles.PathFromRoot(root, file)}: touches {touched}."))
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    [Theory]
    [MemberData(nameof(AccessesInCode))]
    public void MachineAccessIn_AccessInCode_IsFound(string source) => Assert.NotEmpty(MachineAccessIn(source));

    [Theory]
    [MemberData(nameof(LookalikesOfMachineAccess))]
    public void MachineAccessIn_Lookalike_IsNotFound(string source) => Assert.Empty(MachineAccessIn(source));

    private static IEnumerable<string> MachineAccessIn(string source)
    {
        var code = RepositoryFiles.CodeOf(source);

        return Accesses.Where(access => access.Pattern.IsMatch(code)).Select(access => access.Touches);
    }

    // One kind of machine access: what it touches, as a finding states it, and how code spells it.
    public readonly record struct MachineAccess(string Touches, Regex Pattern);
}
