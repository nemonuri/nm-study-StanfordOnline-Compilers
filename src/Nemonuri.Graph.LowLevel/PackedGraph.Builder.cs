
using Nemonuri.LowLevel.Abstractions;

namespace Nemonuri.Graph.LowLevel;

public readonly partial struct PackedGraph<TNodeKey, TEdgeLabel, TNodeValue, TReceiver> 
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    public struct Builder
    {
        //private readonly DangerousMemoryViewProviderBuilder<ObjectOrPointer,  _dangerousMemoryViewProviderBuilder;
    }
}

public readonly record struct PackedGraphMemoryKey<TNodeKey, TEdgeLabel>
    where TNodeKey : IEquatable<TNodeKey>
    where TEdgeLabel : IEquatable<TEdgeLabel>
{
    //private readonly LowLevelChoice<
}