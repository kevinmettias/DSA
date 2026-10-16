using System.Reflection;
using System.Runtime.CompilerServices;
using DSAExperimentation.Algorithms.Reducing;

namespace DSAExperimentation.Tests.Architecture;

// The solutions and benchmarks compose the library through its public types (ARCHITECTURE
// section 17.6a), and the compiler holds them to that only while the library grants its
// internals to test assemblies alone. One InternalsVisibleTo line naming
// DSAExperimentation.LeetCode or DSAExperimentation.Benchmarks would quietly make every storage
// class, walk engine and visit guard reachable from a solution again, so a reach-in has to become
// a library feature or a type made public on purpose instead.
public sealed partial class InternalsVisibilityTests
{
    private const string TestAssemblySuffix = ".Tests";

    [Fact]
    public void Library_GrantsItsInternals_OnlyToTestAssemblies() =>
        Assert.DoesNotContain(LibraryFriends(), friend => !friend.EndsWith(TestAssemblySuffix, StringComparison.Ordinal));

    // A reader that found no grant at all would pass the test above while checking nothing. This
    // assembly needs its own grant to compile, so it has to be among the ones read.
    [Fact]
    public void LibraryFriends_IncludeThisTestAssembly() =>
        Assert.Contains(typeof(InternalsVisibilityTests).Assembly.GetName().Name, LibraryFriends());

    private static IEnumerable<string> LibraryFriends() =>
        typeof(Reduce).Assembly.GetCustomAttributes<InternalsVisibleToAttribute>().Select(grant => grant.AssemblyName);
}
