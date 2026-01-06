using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace Nemonuri.LowLevel.Primitives.DotNet.UnitTests;

// https://github.com/xunit/xunit/issues/1378#issuecomment-3151096317
public class InlineType<TTestParam> : DataAttribute where TTestParam : new()
{
    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(
        MethodInfo testMethod,
        DisposalTracker disposalTracker) => new([new TheoryDataRow(new TTestParam())]);

    public override bool SupportsDiscoveryEnumeration() => true;
}